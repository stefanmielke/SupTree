using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SupTree.Common;

namespace SupTree.RabbitMQ
{
    public class MessageQueueRabbitMQSubscriber : IMessageReceiver
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queue;

        public MessageQueueRabbitMQSubscriber(string hostname, string exchange, string queue)
        {
            _queue = queue;

            var factory = new ConnectionFactory { HostName = hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange, "fanout", true);
            _channel.QueueDeclare(queue, true, false, false);
            _channel.QueueBind(queue, exchange, "");
        }

        public Message Receive()
        {
            BasicGetResult message;
            do
            {
                message = _channel.BasicGet(_queue, true);
                if (message == null)
                    Thread.Sleep(10);
            } while (message == null);

            var bytes = message.Body;
            var unzipedContent = Compression.UnGZip(bytes, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Message>(unzipedContent);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
