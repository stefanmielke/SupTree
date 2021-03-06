﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Funq;

namespace SupTree
{
    public class Supervisor : ISupervisor
    {
        public SupervisorConfiguration Configuration { get; private set; }
        public Container Container { get; }

        private readonly IMessageReceiver _receiver;
        private bool _finished;

        private List<Thread> _threads;

        public Supervisor(Container container, SupervisorConfiguration configuration)
        {
            _receiver = container.Resolve<IMessageReceiver>();
            Container = container;
            Configuration = configuration;
        }

        private Message GetMessage()
        {
            Message message;
            do
            {
                message = _receiver.Receive();
            } while (message == null);

            return message;
        }

        public void Start()
        {
            _finished = false;

            _threads = new List<Thread>(Configuration.MaxWorkers);

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
                Thread.Sleep(Configuration.WaitFreeThreadTime);

            _threads.Clear();
        }

        private void WaitFreeThread()
        {
            while (_threads.Count >= Configuration.MaxWorkers)
            {
                Thread.Sleep(Configuration.WaitFreeThreadTime);

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
                    case SystemMessage.MessageType.ChangeConfigurationMaxWorkers:
                        {
                            if (int.TryParse(systemMessage.Value, out int maxWorkers))
                            {
                                var config = Configuration;
                                config.MaxWorkers = maxWorkers;
                                Configuration = config;
                            }
                        }
                        break;
                    case SystemMessage.MessageType.ChangeConfigurationWaitFreeThreadTime:
                        {
                            if (int.TryParse(systemMessage.Value, out int maxWaitTime))
                            {
                                var config = Configuration;
                                config.WaitFreeThreadTime = maxWaitTime;
                                Configuration = config;
                            }
                        }
                        break;
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
            var worker = Container.Resolve<IWorker>();
            worker?.Work(this, message);
        }
    }
}
