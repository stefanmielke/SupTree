using System;
using System.Text;
using Newtonsoft.Json;
using SupTree.Common;
using ZeroMQ;

namespace SupTree.ZeroMQ
{
    public interface IMessageQueueZeroMQPublisher : IMessageSender
    {
    }

    public class MessageQueueZeroMQPublisher : IMessageQueueZeroMQPublisher
    {
        private readonly string _endpoint;

        private static readonly object Blocker = new object();
        private readonly ZSocket _requester;

        public MessageQueueZeroMQPublisher(string endpoint)
        {
            _endpoint = endpoint;
            _requester = new ZSocket(ZSocketType.PUB);
            _requester.Bind(_endpoint);
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            lock (Blocker)
            {
                using (var zmessage = new ZMessage())
                {
                    zmessage.Add(new ZFrame(message.Tag ?? ""));
                    zmessage.Add(new ZFrame(messageGziped));

                    _requester.Send(zmessage);
                }
            }
        }

        public void Dispose()
        {
            _requester.Dispose();
        }
    }
}
