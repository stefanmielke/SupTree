using System.Text;
using Newtonsoft.Json;
using SupTree.Common;
using ZeroMQ;

namespace SupTree.ZeroMQ
{
    public class MessageQueueZeroMQReceiver : IMessageReceiver
    {
        private readonly string _endpoint;
        private readonly ZSocket _receiver;

        public MessageQueueZeroMQReceiver(string endpoint)
        {
            _endpoint = endpoint;
            _receiver = new ZSocket(ZSocketType.PULL);
            _receiver.Bind(endpoint);
        }

        public Message Receive()
        {
            using (var request = _receiver.ReceiveFrame())
            {
                var bytes = request.Read();

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
