using System.Text;
using Newtonsoft.Json;
using SupTree.Common;
using ZeroMQ;

namespace SupTree.ZeroMQ
{
    public class MessageQueueZeroMQSubscriber : IMessageReceiver
    {
        private readonly string _endpoint;
        private readonly ZSocket _receiver;
        private readonly ZContext _context;

        public MessageQueueZeroMQSubscriber(string endpoint, string filter)
        {
            _endpoint = endpoint;
            _context = new ZContext();
            _receiver = new ZSocket(_context, ZSocketType.SUB);
            _receiver.Connect(endpoint);
            _receiver.Subscribe(filter);
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
            _context.Dispose();
        }
    }
}
