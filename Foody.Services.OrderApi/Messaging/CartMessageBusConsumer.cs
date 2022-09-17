using AutoMapper;
using Foody.MessageBus;
using Foody.Services.OrderApi.Messages;
using Foody.Services.OrderApi.Models.DataTransferObjs;
using Foody.Services.OrderApi.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Foody.Services.OrderApi.Messaging
{
    public class CartMessageBusConsumer : BackgroundService
    {
        private readonly OrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;
        private IMapper _mapper;
        private IMessageBus _messageBus;
        private IConfiguration _configuration;

        private readonly string CheckoutMessageQueue;
        private readonly string PaymentRequestQueue;
        private readonly string PaymentStatusUpdateQueue;
        public CartMessageBusConsumer(OrderRepository orderRepository,IConfiguration configuration,IMapper mapper,IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _configuration = configuration;
            _messageBus = messageBus;
            CheckoutMessageQueue = _configuration.GetValue<string>("QueueConfiguration:CheckoutMessageQueue");
            PaymentRequestQueue = _configuration.GetValue<string>("QueueConfiguration:PaymentRequestQueue");
            PaymentStatusUpdateQueue = _configuration.GetValue<string>("QueueConfiguration:PaymentStatusUpdate");

            //can use existing connection
            if (GetConnection())
            {
                //create channel to get msg from RMQ, channel is created as model in .net
                _channel = _connection.CreateModel();
                //read queue name from configuration file or static file
                _channel.QueueDeclare(queue: CheckoutMessageQueue, false, false, false, arguments: null);
            }          
        }

        public bool GetConnection()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();
            return _connection != null;
        }

        public void CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            //create connection using the factory object

            _connection = factory.CreateConnection();
        }

        //this method listens to messages in the queue
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested(); //stop if cancelled

            //creat and configure consumer 
            var consumer = new EventingBasicConsumer(_channel);
            //configue received event handler
            //the message will be in the event body
            consumer.Received += (channel, evnt) =>
            {
                var content = Encoding.UTF8.GetString(evnt.Body.ToArray());
                OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(content);

                HandleMessage(orderHeaderDto).GetAwaiter().GetResult();
                //send ack to channel to delete the message
                _channel.BasicAck(evnt.DeliveryTag, false);
            };

            //consume msg from queue
            _channel.BasicConsume(CheckoutMessageQueue, false, consumer);
            
            return Task.CompletedTask;
        }

        private async Task HandleMessage(OrderHeaderDto orderHeaderDto)
        {
            OrderDto fullOrderDto = _mapper.Map<OrderDto>(orderHeaderDto);
            fullOrderDto.OrderTime = DateTime.Now;
            fullOrderDto.PaymentStatus = false;
            fullOrderDto.OrderDetails = new();
            foreach(var detail in orderHeaderDto.CartDetails)
            {
                OrderDetailDto orderDetailDto = new();
                orderDetailDto.ProductId = detail.ProductId;
                orderDetailDto.ProductName = detail.Product.Name;
                orderDetailDto.Price = detail.Product.Price;
                orderDetailDto.Count = detail.Count;
                fullOrderDto.TotalItems += detail.Count;
                fullOrderDto.OrderDetails.Add(orderDetailDto);
            }
            var newPlacedOrder = await _orderRepository.AddOrdertoDatabase(fullOrderDto);
            
            PaymentRequestDto paymentRequestDto = new PaymentRequestDto();
            paymentRequestDto.OrderId = newPlacedOrder.OrderHeaderId;
            paymentRequestDto.OrderTotal = fullOrderDto.OrderTotal;
            paymentRequestDto.Name = fullOrderDto.FirstName + fullOrderDto.LastName;
            paymentRequestDto.CardNumber = fullOrderDto.CardNumber;
            paymentRequestDto.CVV = fullOrderDto.CVV;
            paymentRequestDto.ExpiryMonthYear = fullOrderDto.ExpiryMonthYear;

            try
            {                
                _messageBus.SendMessage(paymentRequestDto, PaymentRequestQueue);
            }
            catch (Exception)
            {

            }

        }
    }
}
