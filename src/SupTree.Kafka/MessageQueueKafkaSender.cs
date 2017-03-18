using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;
using SupTree.Common;

namespace SupTree.Kafka
{
    public class MessageQueueKafkaSender : IMessageSender, IDisposable
    {
        private readonly string _topic;
        private readonly Producer<string, string> _producer;

        public MessageQueueKafkaSender(string broker, string topic)
        {
            _topic = topic;
            var config = new Dictionary<string, object> { { "bootstrap.servers", broker }, { "api.version.request", true }, { "queue.buffering.max.ms", 1 } };

            _producer = new Producer<string, string>(config, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);
            messageContent = Encoding.UTF8.GetString(messageGziped);

            var result = _producer.ProduceAsync(_topic, "ky", "testing").Result;
        }

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
