namespace SupTree
{
    public interface IWorker
    {
        void Work(ISupervisor supervisor, Message message);
    }
}