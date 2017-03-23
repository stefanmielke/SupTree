using Funq;

namespace SupTree
{
    public interface ISupervisor
    {
        void Start();
        Container Container { get; }
    }
}