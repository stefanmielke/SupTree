using System;
using System.Messaging;

namespace SupTree.MSMQ
{
    public interface IMessageQueueMSMQ : IMessageReceiver, IMessageSender { }

    public class MessageQueueMSMQ : IMessageQueueMSMQ
    {
        private readonly MessageQueue _queue;

        public MessageQueueMSMQ(string queueName)
        {
            _queue = GetQueue(queueName);
        }

        public Message Receive()
        {
            System.Messaging.Message queueMessage;
            do
            {
                queueMessage = _queue.Receive();
            } while (queueMessage == null);

            return queueMessage.Body as Message;
        }

        public void Send(Message message)
        {
            var queueMessage = new System.Messaging.Message(message) { Formatter = new JsonMessageFormatter() };

            _queue.Send(queueMessage);
        }

        private static MessageQueue GetQueue(string queueName)
        {
            var completQueueName = string.Concat(Environment.MachineName, @"\private$\", queueName);

            var queue = new MessageQueue(completQueueName) { Formatter = new JsonMessageFormatter() };

            if (!MessageQueue.Exists(queue.Path))
                MessageQueue.Create(queue.Path);

            return queue;
        }

        public void Dispose()
        {
            _queue.Dispose();
        }
    }
}
