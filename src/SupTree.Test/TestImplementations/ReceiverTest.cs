using System.Collections.Generic;

namespace SupTree.Test.TestImplementations
{
    class ReceiverTest : IMessageReceiver
    {
        public readonly Queue<Message> MessageQueue = new Queue<Message>();

        public void CreateMessage()
        {
            var message = new Message
            {
                Format = "TEST"
            };

            MessageQueue.Enqueue(message);
        }

        public void CreateSystemMessage(SystemMessage.MessageType configType, string value = null)
        {
            var message = new Message { Format = "SYSTEM" };

            message.SetBody(new SystemMessage { Type = configType, Value = value});

            MessageQueue.Enqueue(message);
        }

        public Message Receive()
        {
            while (MessageQueue.Count == 0) ;

            return MessageQueue.Dequeue();
        }
    }
}
