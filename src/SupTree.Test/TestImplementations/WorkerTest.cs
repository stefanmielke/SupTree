﻿using System;
using System.Threading;

namespace SupTree.Test.TestImplementations
{
    class WorkerTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            return true;
        }

        protected override void OnSuccess()
        {
            base.OnSuccess();

            Supervisor.SendMessage(new Message { Format = "SUCCESS" });
        }
    }

    class WorkerFailureTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            return false;
        }

        protected override void OnFailure()
        {
            base.OnFailure();

            Supervisor.SendMessage(new Message { Format = "FAILURE" });
        }
    }

    class WorkerErrorTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            throw new Exception("Test error");
        }

        protected override void OnError(Exception exception)
        {
            base.OnError(exception);

            Supervisor.SendMessage(new Message { Format = "ERROR" });
        }
    }

    class WorkerOverloadTest : Worker
    {
        protected override bool DoWork(Message message)
        {
            Thread.Sleep(10000);

            return true;
        }
    }
}
