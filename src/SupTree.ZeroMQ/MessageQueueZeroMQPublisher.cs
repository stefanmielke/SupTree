using System;
using System.Text;
using Newtonsoft.Json;
using SupTree.Common;
using ZeroMQ;

namespace SupTree.ZeroMQ
{
    public class MessageQueueZeroMQPublisher : IMessageSender
    {
        private readonly string _endpoint;

        private static readonly object Blocker = new object();

        public MessageQueueZeroMQPublisher(string endpoint)
        {
            _endpoint = endpoint;
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            lock (Blocker)
            {
                using (var context = new ZContext())
                {
                    using (var requester = new ZSocket(context, ZSocketType.PUSH))
                    {
                        requester.Bind(_endpoint);

                        using (var zmessage = new ZMessage())
                        {
                            zmessage.Add(new ZFrame(message.Tag));
                            zmessage.Add(new ZFrame(messageGziped));

                            requester.Send(zmessage);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
