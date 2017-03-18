using System.Collections.Generic;

namespace SupTree.Test.TestImplementations
{
    class SenderTest : IMessageSender
    {
        public List<Message> Messages { get; }

        public SenderTest()
        {
            Messages = new List<Message>();
        }

        public void Send(Message message)
        {
            Messages.Add(message);
        }
    }
}
