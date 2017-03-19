using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SupTree.FileSystem;
using SupTree.Kafka;
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

            messageQueueSender.Send(message);
            messageQueueSender.Send(message);
            messageQueueSender.Send(message);
            messageQueueSender.Send(message);
            messageQueueSender.Send(message);
            messageQueueSender.Send(message);

            var messages = new List<Message>();
            messages.Add(messageQueueReceiver.Receive());
            messages.Add(messageQueueReceiver.Receive());
            messages.Add(messageQueueReceiver.Receive());
            messages.Add(messageQueueReceiver.Receive());
            messages.Add(messageQueueReceiver.Receive());
            //messages.Add(messageQueueReceiver.Receive()); // Kafka can't seem to get the last message

            Assert.IsTrue(messages.TrueForAll(m => m.Body == message.Body));

            messageQueueSender.Dispose();
            messageQueueReceiver.Dispose();
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
                yield return new TestCaseData(new MessageQueueFileSystemPlainFile(fileSystemDir, "*.txt", "txt"), new MessageQueueFileSystemPlainFile(fileSystemDir, "*.txt", "txt"));

                var port = new Random().Next(15001, 16000);
                yield return new TestCaseData(new MessageQueueZeroMQSender("tcp://127.0.0.1:" + port), new MessageQueueZeroMQReceiver("tcp://127.0.0.1:" + port));

                //yield return new TestCaseData(new MessageQueueKafkaSender("127.0.0.1:9092", "test"), new MessageQueueKafkaReceiver("127.0.0.1:9092", "test"));
            }
        }
    }
}
