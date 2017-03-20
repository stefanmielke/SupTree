using System;

namespace SupTree
{
    public abstract class Worker : IWorker
    {
        protected ISupervisor Supervisor;
        private Exception _lastException;

        public void Work(ISupervisor supervisor, Message message)
        {
            Supervisor = supervisor;

            var result = DoWorkInternal(message);
            switch (result)
            {
                case Result.Failure:
                    OnFailure();
                    break;
                case Result.Error:
                    OnError(_lastException);
                    break;
                default:
                    OnSuccess();
                    break;
            }
        }

        private Result DoWorkInternal(Message message)
        {
            try
            {
                return DoWork(message) ? Result.Success : Result.Failure;
            }
            catch (Exception ex)
            {
                _lastException = ex;
                return Result.Error;
            }
        }

        protected abstract bool DoWork(Message message);

        protected virtual void OnSuccess()
        {
        }

        protected virtual void OnFailure()
        {
        }

        protected virtual void OnError(Exception exception)
        {
        }
    }
}
