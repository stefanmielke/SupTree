using System;
using System.Collections;
using System.Collections.Generic;
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
        public void CanSendAndReceiveMessage(IMessageSender messageQueueSender, IMessageReceiver messageQueueReceiver)
        {
            var message = GetNewMessage();

            messageQueueSender.Send(message);

            var receivedMessage = messageQueueReceiver.Receive();

            Assert.That(receivedMessage.Body, Is.EqualTo(message.Body));

            messageQueueSender.Dispose();
            messageQueueReceiver.Dispose();
        }

        [Test, TestCaseSource(typeof(MessagingTestDataFactory), nameof(MessagingTestDataFactory.TestCases))]
        public void CanSendAndReceiveMessages(IMessageSender messageQueueSender, IMessageReceiver messageQueueReceiver)
        {
            var message = GetNewMessage();

            for (int i = 0; i < 200; i++)
            {
                messageQueueSender.Send(message); 
            }

            var messages = new List<Message>();
            for (int i = 0; i < 200; i++)
            {
                messages.Add(messageQueueReceiver.Receive()); 
            }

            Assert.IsTrue(messages.TrueForAll(m => m.Body == message.Body));

            messageQueueSender.Dispose();
            messageQueueReceiver.Dispose();
        }

        private static Message GetNewMessage()
        {
            var message = new Message { Tag = "A" };
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
                yield return new TestCaseData(new MessageQueueFileSystemPlainFile(fileSystemDir, "*.txt", "txt"), new MessageQueueFileSystemPlainFile(fileSystemDir, "*.txt", "txt"));

                var random = new Random();

                var port = random.Next(15001, 16000);
                yield return new TestCaseData(new MessageQueueZeroMQSender("tcp://127.0.0.1:" + port), new MessageQueueZeroMQReceiver("tcp://127.0.0.1:" + port));

                var port2 = random.Next(15001, 16000);
                var pub = new MessageQueueZeroMQPublisher("tcp://*:" + port2);
                var sub = new MessageQueueZeroMQSubscriber("tcp://127.0.0.1:" + port2, "A");

                yield return new TestCaseData(pub, sub);

                //yield return new TestCaseData(new MessageQueueKafkaSender("127.0.0.1:9092", "test"), new MessageQueueKafkaReceiver("127.0.0.1:9092", "test"));
            }
        }
    }
}
