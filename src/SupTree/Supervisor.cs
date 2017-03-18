using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StructureMap;

namespace SupTree
{
    public class Supervisor
    {
        private readonly IMessageReceiver _receiver;
        private readonly IMessageSender _sender;
        private readonly Func<Worker> _factory;
        private readonly SupervisorConfiguration _configuration;
        private bool _finished;

        private List<Thread> _threads;

        public Supervisor(IContainer container, Func<Worker> factoryMethod, SupervisorConfiguration configuration)
        {
            _receiver = container.GetInstance<IMessageReceiver>();
            _sender = container.GetInstance<IMessageSender>();
            _factory = factoryMethod;
            _configuration = configuration;
        }

        private Message GetMessage()
        {
            return _receiver.Receive();
        }

        public void SendMessage(Message message)
        {
            _sender.Send(message);
        }

        public void Start()
        {
            _finished = false;

            _threads = new List<Thread>(_configuration.MaxWorkersOverload);

            while (!_finished)
            {
                var message = GetMessage();
                switch (message.Format)
                {
                    case "SYSTEM":
                        HandleSystemMessage(message);
                        break;
                    default:
                        SendToWorker(message);
                        break;
                }

                CleanDoneWorkers();

                WaitFreeThread();
            }

            while (_threads.Any(t => t.IsAlive))
                Thread.Sleep(10);

            _threads.Clear();
        }

        private void WaitFreeThread()
        {
            while (_threads.Count >= _configuration.MaxWorkers)
            {
                Thread.Sleep(1);

                CleanDoneWorkers();
            }
        }

        private void CleanDoneWorkers()
        {
            _threads.RemoveAll(t => !t.IsAlive);
        }

        private void HandleSystemMessage(Message message)
        {
            var systemMessage = message.GetBody<SystemMessage>();
            if (systemMessage != null)
            {
                switch (systemMessage.Type)
                {
                    default:
                        _finished = true;
                        break;
                }
            }
        }

        private void SendToWorker(Message message)
        {
            var thread = new Thread(() => { StartWorker(message); });
            thread.Start();

            _threads.Add(thread);
        }

        private void StartWorker(Message message)
        {
            var worker = _factory();
            worker?.Work(this, message);
        }
    }
}
