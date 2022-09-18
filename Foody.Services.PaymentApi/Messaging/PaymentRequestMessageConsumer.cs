using Foody.Services.PaymentApi.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Foody.Services.PaymentApi.Messaging
{
    internal class PaymentRequestMessageConsumer : BackgroundService
    {
        private readonly IProcessPayment _processPayment;
        private IConnection _connection;
        private IModel _channel;
        private IMessageBus _messageBus;
        private IConfiguration _configuration;

        private readonly string PaymentRequestMessageQueue;
        private readonly string PaymentStatusMessageQueue;


        public PaymentRequestMessageConsumer(IProcessPayment processPayment, IMessageBus messageBus,IConfiguration configuration)
        {
            _processPayment = processPayment;
            _configuration = configuration;

            PaymentRequestMessageQueue = _configuration.GetValue<string>("QueueConfiguration:PaymentRequestQueue");
            PaymentStatusMessageQueue = _configuration.GetValue<string>("QueueConfiguration:PaymentStatusUpdate");
            //can use existing connection
            if (GetConnection())
            {
                //create channel to get msg from RMQ, channel is created as model in .net
                _channel = _connection.CreateModel();
                //read queue name from configuration file or static file
                _channel.QueueDeclare(queue: PaymentRequestMessageQueue, false, false, false, arguments: null);
            }
            _messageBus = messageBus;

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
                PaymentRequestDto paymentRequestDto = JsonConvert.DeserializeObject<PaymentRequestDto>(content);

                HandleMessage(paymentRequestDto).GetAwaiter().GetResult();
                //send ack to channel to delete the message
                _channel.BasicAck(evnt.DeliveryTag, false);
            };

            //consume msg from queue
            _channel.BasicConsume(PaymentRequestMessageQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentRequestDto paymentRequestDto)
        {
            PaymentStatusDto paymentStatusDto = new()
            {
                OrderId = paymentRequestDto.OrderId,
                IsConfirmed = _processPayment.PaymentProcessor(),
                UserEmail = paymentRequestDto.UserEmail
            };

            //publish payment status to bus and also for email service
            _messageBus.PublishMessage(paymentStatusDto);

        }

    }

}
