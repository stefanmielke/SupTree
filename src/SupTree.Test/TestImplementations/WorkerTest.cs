using System;
using System.Threading;

namespace SupTree.Test.TestImplementations
{
    internal class WorkerTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            return true;
        }

        protected override void OnSuccess()
        {
            base.OnSuccess();

            using (var sender = Supervisor.Container.Resolve<IMessageSender>())
                sender.Send(new Message { Format = "SUCCESS" });
        }
    }

    internal class WorkerFailureTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            return false;
        }

        protected override void OnFailure()
        {
            base.OnFailure();

            using (var sender = Supervisor.Container.Resolve<IMessageSender>())
                sender.Send(new Message { Format = "FAILURE" });
        }
    }

    internal class WorkerErrorTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            throw new Exception("Test error");
        }

        protected override void OnError(Exception exception)
        {
            base.OnError(exception);

            using (var sender = Supervisor.Container.Resolve<IMessageSender>())
                sender.Send(new Message { Format = "ERROR" });
        }
    }

    internal class WorkerOverloadTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            Thread.Sleep(10000);

            return true;
        }
    }
}
