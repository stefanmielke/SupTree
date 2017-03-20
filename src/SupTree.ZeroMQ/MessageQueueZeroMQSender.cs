using System;
using System.Text;
using Newtonsoft.Json;
using SupTree.Common;
using ZeroMQ;

namespace SupTree.ZeroMQ
{
    public interface IMessageQueueZeroMQSender : IMessageSender { }

    public class MessageQueueZeroMQSender : IMessageQueueZeroMQSender
    {
        private readonly ZSocket _requester;

        public MessageQueueZeroMQSender(string endpoint)
        {
            _requester = new ZSocket(ZSocketType.PUSH);
            _requester.Connect(endpoint);
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            _requester.Send(new ZFrame(messageGziped));
        }

        public void Dispose()
        {
            _requester.Dispose();
        }
    }
}
