using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;
using SupTree.Common;

namespace SupTree.Kafka
{

    public class MessageQueueKafkaReceiver : IMessageReceiver, IDisposable
    {
        private readonly Consumer<string, string> _consumer;
        private readonly Queue<Message<string, string>> _messages = new Queue<Message<string, string>>();

        public MessageQueueKafkaReceiver(string broker, string topic)
        {
            var config = new Dictionary<string, object> { { "group.id", "mq-kafka-consumer" }, { "bootstrap.servers", broker }, { "api.version.request", true } };

            _consumer = new Consumer<string, string>(config, new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8));
            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topic, 0, 0) });
            _consumer.Subscribe(topic);
            _consumer.OnMessage += ConsumeMessage;
        }

        public Message Receive()
        {
            while (_messages.Count == 0)
                _consumer.Poll();

            var msg = _messages.Dequeue();

            var bytes = Encoding.UTF8.GetBytes(msg.Value);

            var unzipedContent = Compression.UnGZip(bytes, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Message>(unzipedContent);
        }

        private void ConsumeMessage(object sender, Message<string, string> message)
        {
            _messages.Enqueue(message);
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}
