using System.Collections.Generic;

namespace SupTree.Test.TestImplementations
{
    class ReceiverTest : IMessageReceiver
    {
        public readonly Queue<Message> MessageQueue = new Queue<Message>();

        public void CreateStopMessage()
        {
            var message = new Message
            {
                Format = "SYSTEM"
            };

            message.SetBody(new SystemMessage { Type = SystemMessage.MessageType.Stop });

            MessageQueue.Enqueue(message);
        }

        public void CreateMessage()
        {
            var message = new Message
            {
                Format = "TEST"
            };

            MessageQueue.Enqueue(message);
        }

        public Message Receive()
        {
            while (MessageQueue.Count == 0) ;

            return MessageQueue.Dequeue();
        }
    }
}
