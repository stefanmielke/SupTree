using System;
using System.Collections;
using NUnit.Framework;
using SupTree.FileSystem;
using SupTree.MSMQ;
using SupTree.Test.TestImplementations;
using SupTree.ZeroMQ;

namespace SupTree.Test
{
    [TestFixture]
    internal class MessagingTests
    {
        [Test, TestCaseSource(typeof(MessagingTestDataFactory), nameof(MessagingTestDataFactory.TestCases))]
        public void CanSendAndReceiveMessages(IMessageSender messageQueueSender, IMessageReceiver messageQueueReceiver)
        {
            var message = GetNewMessage();

            messageQueueSender.Send(message);

            var receivedMessage = messageQueueReceiver.Receive();

            Assert.That(receivedMessage.Body, Is.EqualTo(message.Body));
        }

        private static Message GetNewMessage()
        {
            var message = new Message();
            message.SetBody(new SimpleMessageObject { Guid = new Guid().ToString() });

            return message;
        }
    }

    public class MessagingTestDataFactory
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new MessageQueueMSMQ("test"), new MessageQueueMSMQ("test"));

                var fileSystemDir = @"C:\Users\mielke\Desktop\test\" + Guid.NewGuid();
                yield return new TestCaseData(new MessageQueueFileSystem(fileSystemDir, "*.json", "json"), new MessageQueueFileSystem(fileSystemDir, "*.json", "json"));

                yield return new TestCaseData(new MessageQueueZeroMQSender("tcp://127.0.0.1:15055"), new MessageQueueZeroMQReceiver("tcp://127.0.0.1:15055"));
            }
        }
    }
}
