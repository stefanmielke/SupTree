using System;
using System.Text;
using Newtonsoft.Json;
using SupTree.Common;
using ZeroMQ;

namespace SupTree.ZeroMQ
{
    public class MessageQueueZeroMQSender : IMessageSender
    {
        private readonly string _endpoint;

        public MessageQueueZeroMQSender(string endpoint)
        {
            _endpoint = endpoint;
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            using (var requester = new ZSocket(ZSocketType.PUSH))
            {
                requester.Connect(_endpoint);

                requester.Send(new ZFrame(messageGziped));
            }
        }

        public void Dispose()
        {
        }
    }
}
