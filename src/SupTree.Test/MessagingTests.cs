using System;
using NUnit.Framework;
using SupTree.MSMQ;
using SupTree.Test.TestImplementations;

namespace SupTree.Test
{
    [TestFixture]
    internal class MessagingTests
    {
        [Test]
        public void CanSendAndReceiveMessage()
        {
            var messageQueue = new MessageQueueMSMQ("test");

            var message = GetNewMessage();

            messageQueue.Send(message);

            var receivedMessage = messageQueue.Receive();

            Assert.That(receivedMessage.Body, Is.EqualTo(message.Body));
        }

        private static Message GetNewMessage()
        {
            var message = new Message();
            message.SetBody(new SimpleMessageObject { Guid = new Guid().ToString() });

            return message;
        }
    }
}
