using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using SupTree.Common;

namespace SupTree.Kafka
{
    public interface IMessageQueueKafkaReceiver : IMessageReceiver { }

    public class MessageQueueKafkaReceiver : IMessageQueueKafkaReceiver
    {
        private readonly Consumer<Null, byte[]> _consumer;
        private readonly Queue<Message<Null, byte[]>> _messages = new Queue<Message<Null, byte[]>>();

        public MessageQueueKafkaReceiver(string broker, string topic)
        {
            var config = new Dictionary<string, object> { { "group.id", "mq-kafka-consumer" }, { "bootstrap.servers", broker }, { "api.version.request", true } };

            _consumer = new Consumer<Null, byte[]>(config, null, new ByteArrayFormatter());
            _consumer.Subscribe(topic);
            _consumer.OnMessage += ConsumeMessage;
        }

        public Message Receive()
        {
            while (_messages.Count == 0)
                _consumer.Poll();

            var msg = _messages.Dequeue();

            var bytes = msg.Value;

            var unzipedContent = Compression.UnGZip(bytes, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Message>(unzipedContent);
        }

        private void ConsumeMessage(object sender, Message<Null, byte[]> message)
        {
            _messages.Enqueue(message);
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}
