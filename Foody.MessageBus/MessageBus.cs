using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Foody.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;

        public MessageBus()
        {
            _hostname = "localhost";
            _username = "user1";
            _password = "password1!";
        }
        public void SendMessage(BaseMessage message, string queueName)
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
            //create channel to send msg to RMQ

            using var channel = _connection.CreateModel(); //this establishes a connection and returns a channel, session and model
            
            //configure the type of channel (standard, direct, publish or subscribe etc)
            //durable of the queue defines if the queue survives the server restart, its not for the messages
            //setting all to false for getting a queue with basic configuration
            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);

            //send message to the declared queue
            //serialize the msg object and encode it to bytes
            var jsonMsg = JsonConvert.SerializeObject(message);
            var msgBody = Encoding.UTF8.GetBytes(jsonMsg);

            //publish the message to channel
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: msgBody);
        }
    }
}
