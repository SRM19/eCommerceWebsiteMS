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
        //private const string ExchangeName = "PubSubPaymentUpdate_Exchange";
        //string queueName = "";
        private const string Direct_Exchange = "PubSubDirect_Exchange";
        private readonly string PaymentStatusQueue = "paymentstatusqueue";
        private readonly string PaymentRouteKey = "paymentstatus";

        public PaymentMessageBusConsumer(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;           

            //can use existing connection
            if (GetConnection())
            {
                //create channel to get msg from RMQ, channel is created as model in .net
                _channel = _connection.CreateModel();
                //declare exchange with the same name
                //_channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType.Fanout, false, false, arguments: null);
                _channel.ExchangeDeclare(Direct_Exchange, ExchangeType.Direct, false, false, arguments: null);
                _channel.QueueDeclare(PaymentStatusQueue, false, false, false, null);
                //bind a queue to listen to exchange, queue can be declared on the fly, queue can be created with custom name
                //queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(PaymentStatusQueue, Direct_Exchange, PaymentRouteKey);
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
            _channel.BasicConsume(PaymentStatusQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentStatusDto paymentStatusDto)
        {
            await _orderRepository.UpdatePaymentStatus(paymentStatusDto.OrderId, paymentStatusDto.IsConfirmed);

        }
    }
}
