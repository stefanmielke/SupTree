using Ninject.Syntax;

namespace SupTree
{
    public interface ISupervisor
    {
        void Start();
        IResolutionRoot Container { get; }
    }
}