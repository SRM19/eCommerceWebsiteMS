using Foody.MessageBus;
using Foody.Services.Email.Messages;
using Foody.Services.Email.Repository;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace Foody.Services.Email.Messaging
{
    public class EmailMessageBusConsumer : BackgroundService
    {
        private readonly EmailRepository _emailRepository;
        private IConnection _connection;
        private IModel _channel;
        private IConfiguration _configuration;
        //private const string ExchangeName = "PubSubPaymentUpdate_Exchange";
        private const string Direct_Exchange = "PubSubDirect_Exchange";
        private readonly string EmailUpdateQueue = "emailupdatequeue";
        private readonly string EmailRouteKey = "emailupdate";
        //string queueName = "";

        public EmailMessageBusConsumer(EmailRepository emailRepository, IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;

            //can use existing connection
            if (GetConnection())
            {
                //create channel to get msg from RMQ, channel is created as model in .net
                _channel = _connection.CreateModel();
                //declare exchange with the same name
                //_channel.ExchangeDeclare(exchange: ExchangeName,ExchangeType.Fanout, false, false, arguments: null);
                //Direct exchange
                _channel.ExchangeDeclare(Direct_Exchange, ExchangeType.Direct, false, false, arguments: null);
                _channel.QueueDeclare(EmailUpdateQueue, false, false, false, null);
                //bind a queue to listen to exchange, queue can be declared on the fly, queue can be created with custom name
                //queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(EmailUpdateQueue, Direct_Exchange, EmailRouteKey);
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
            _channel.BasicConsume(EmailUpdateQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentStatusDto paymentStatusDto)
        {
            try
            {
                await _emailRepository.SendandLogEmail(paymentStatusDto);

            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
