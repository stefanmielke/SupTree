using System.Text;
using Newtonsoft.Json;
using SupTree.Common;
using ZeroMQ;

namespace SupTree.ZeroMQ
{
    public interface IMessageQueueZeroMQSubscriber : IMessageReceiver { }

    public class MessageQueueZeroMQSubscriber : IMessageQueueZeroMQSubscriber
    {
        private readonly string _endpoint;
        private readonly ZSocket _receiver;

        public MessageQueueZeroMQSubscriber(string endpoint, string filter)
        {
            _endpoint = endpoint;
            _receiver = new ZSocket(ZSocketType.SUB);
            _receiver.Connect(endpoint);
            _receiver.Subscribe(filter);
        }

        public Message Receive()
        {
            using (var message = _receiver.ReceiveMessage())
            {
                if (message.Count <=  1)
                    return new Message();

                var bytes = message[1].Read();
                var unzipedContent = Compression.UnGZip(bytes, Encoding.UTF8);
                return JsonConvert.DeserializeObject<Message>(unzipedContent); 
            }
        }

        public void Dispose()
        {
            _receiver.Disconnect(_endpoint);
            _receiver.Dispose();
        }
    }
}
