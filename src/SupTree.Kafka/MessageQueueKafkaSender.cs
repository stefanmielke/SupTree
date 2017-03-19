using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using SupTree.Common;

namespace SupTree.Kafka
{
    public class MessageQueueKafkaSender : IMessageSender, IDisposable
    {
        private readonly string _topic;
        private readonly Producer<Null, byte[]> _producer;

        public MessageQueueKafkaSender(string broker, string topic)
        {
            _topic = topic;
            var config = new Dictionary<string, object>
            {
                {"bootstrap.servers", broker},
                {"api.version.request", true},
                {"queue.buffering.max.ms", 1},
                {"queue.buffering.max.messages", 1}
            };

            _producer = new Producer<Null, byte[]>(config, null, new ByteArrayFormatter());
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            var result = _producer.ProduceAsync(_topic, null, messageGziped);
            result.ContinueWith(task =>
            {
                //if (task.Result.Error.HasError)
                //    // log error
            });
        }

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
