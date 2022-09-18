using Foody.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Foody.Services.PaymentApi
{
    public class MessageBus : IMessageBus
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        //private const string ExchangeName = "PubSubPaymentUpdate_Exchange";
        private const string Direct_Exchange = "PubSubDirect_Exchange";
        private readonly string EmailUpdateQueue = "emailupdatequeue";
        private readonly string PaymentStatusQueue = "paymentstatusqueue";

        public MessageBus()
        {
            _hostname = "localhost";
            _username = "guest";//user1
            _password = "guest";//Password1!
        }
        public void PublishMessage(BaseMessage message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            //create connection using the factory object

             _connection = factory.CreateConnection();

            //after connection is established, send the msg
            //create channel to send msg to RMQ channel is created as model in .net

            using var channel = _connection.CreateModel(); //this establishes a connection and returns a channel, session and model

            //configure the exchange where message will be sent
            //durable of the exchange defines if the queue survives the server restart, its not for the messages
            //setting all to false for getting a exchange with basic configuration
            //channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, false, false, arguments: null);
            //direct exchange
            channel.ExchangeDeclare(Direct_Exchange, ExchangeType.Direct, false, false, arguments: null);
            channel.QueueDeclare(PaymentStatusQueue, false, false, false, null);
            channel.QueueDeclare(EmailUpdateQueue, false, false, false, null);

            //bind the queue to exchange
            channel.QueueBind(PaymentStatusQueue, Direct_Exchange, "paymentstatus");
            channel.QueueBind(EmailUpdateQueue, Direct_Exchange, "emailupdate");

            //send message to the declared queue
            //serialize the msg object and encode it to bytes
            var jsonMsg = JsonConvert.SerializeObject(message);
            var msgBody = Encoding.UTF8.GetBytes(jsonMsg);

            //publish the message to channel
            // channel.BasicPublish(exchange: ExchangeName, routingKey: "", basicProperties: null, body: msgBody);
            //publish message to direct exchange
            channel.BasicPublish(exchange: Direct_Exchange, "paymentstatus", basicProperties: null, body: msgBody);
            channel.BasicPublish(exchange: Direct_Exchange, "emailupdate", basicProperties: null, body: msgBody);
        }
    }
}
