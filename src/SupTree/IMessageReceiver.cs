using System;

namespace SupTree
{
    public interface IMessageReceiver : IDisposable
    {
        Message Receive();
    }
}
