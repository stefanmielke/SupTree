using System;

namespace SupTree
{
    public interface IMessageSender : IDisposable
    {
        void Send(Message message);
    }
}
