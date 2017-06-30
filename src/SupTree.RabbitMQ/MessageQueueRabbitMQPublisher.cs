using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SupTree.Common;

namespace SupTree.RabbitMQ
{
    public class MessageQueueRabbitMQPublisher : IMessageSender
    {
        private readonly string _exchange;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageQueueRabbitMQPublisher(string hostname, string exchange)
        {
            _exchange = exchange;

            var factory = new ConnectionFactory { HostName = hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange, "fanout", true);
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            _channel.BasicPublish(_exchange, "", null, messageGziped);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
