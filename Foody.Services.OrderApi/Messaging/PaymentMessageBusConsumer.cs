using AutoMapper;
using Foody.MessageBus;
using Foody.Services.OrderApi.Messages;
using Foody.Services.OrderApi.Models.DataTransferObjs;
using Foody.Services.OrderApi.Repository;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace Foody.Services.OrderApi.Messaging
{
    public class PaymentMessageBusConsumer : BackgroundService
    {
        private readonly OrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;
        private IMessageBus _messageBus;
        private IConfiguration _configuration;

        private readonly string PaymentStatusUpdateQueue;
        public PaymentMessageBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;
            PaymentStatusUpdateQueue = _configuration.GetValue<string>("QueueConfiguration:PaymentStatusUpdate");

            //can use existing connection
            if (GetConnection())
            {
                //create channel to get msg from RMQ, channel is created as model in .net
                _channel = _connection.CreateModel();
                //read queue name from configuration file or static file
                _channel.QueueDeclare(queue: PaymentStatusUpdateQueue, false, false, false, arguments: null);
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
                PaymentStatusDto paymentStatusDto = JsonConvert.DeserializeObject<PaymentStatusDto>(content);

                HandleMessage(paymentStatusDto).GetAwaiter().GetResult();
                //send ack to channel to delete the message
                _channel.BasicAck(evnt.DeliveryTag, false);
            };

            //consume msg from queue
            _channel.BasicConsume(PaymentStatusUpdateQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentStatusDto paymentStatusDto)
        {
            await _orderRepository.UpdatePaymentStatus(paymentStatusDto.OrderId, paymentStatusDto.IsConfirmed);

        }
    }
}
