using System;
using System.Threading;
using NUnit.Framework;
using SupTree.Test.TestImplementations;

namespace SupTree.Test
{
    [TestFixture]
    internal class SupervisorTest
    {
        private readonly ReceiverTest _receiver;
        private readonly SenderTest _sender;
        private readonly SupervisorConfiguration _supConfig;

        public SupervisorTest()
        {
            _receiver = new ReceiverTest();
            _sender = new SenderTest();

            _supConfig = new SupervisorConfiguration
            {
                MaxWorkers = 2,
                WaitFreeThreadTime = 1
            };
        }

        [SetUp]
        public void Init()
        {
            _sender.Messages.Clear();
        }

        [Test]
        public void CanReceiveMessage()
        {
            Assert.That(_sender.Messages, Is.Empty);

            var supervisor = new Supervisor(_receiver, _sender, () => new WorkerTest(), _supConfig);
            var supThread = new Thread(() => supervisor.Start());
            supThread.Start();

            _receiver.CreateMessage();

            Thread.Sleep(100);

            Assert.That(_sender.Messages, Has.Count.Positive);
            Assert.IsTrue(_sender.Messages.TrueForAll(m => m.Format == "SUCCESS"));

            supThread.Abort();
            supThread.Join();
        }

        [Test]
        public void CanFailMessage()
        {
            Assert.That(_sender.Messages, Is.Empty);

            var supervisorFailure = new Supervisor(_receiver, _sender, () => new WorkerFailureTest(), _supConfig);
            var supThread = new Thread(() => supervisorFailure.Start());
            supThread.Start();

            _receiver.CreateMessage();

            Thread.Sleep(100);

            Assert.That(_sender.Messages, Has.Count.Positive);
            Assert.IsTrue(_sender.Messages.TrueForAll(m => m.Format == "FAILURE"));

            supThread.Abort();
            supThread.Join();
        }

        [Test]
        public void CanThrowErrorMessage()
        {
            Assert.That(_sender.Messages, Is.Empty);

            var supervisorError = new Supervisor(_receiver, _sender, () => new WorkerErrorTest(), _supConfig);
            var supThread = new Thread(() => supervisorError.Start());
            supThread.Start();

            _receiver.CreateMessage();

            Thread.Sleep(2000);

            Assert.That(_sender.Messages, Has.Count.Positive);
            Assert.IsTrue(_sender.Messages.TrueForAll(m => m.Format == "ERROR"));

            supThread.Abort();
            supThread.Join();
        }

        [Test]
        public void CanReceiveStopMessage()
        {
            Assert.That(_sender.Messages, Is.Empty);

            var supervisor = new Supervisor(_receiver, _sender, () => new WorkerTest(), _supConfig);
            var supThread = new Thread(() => supervisor.Start());
            supThread.Start();

            _receiver.CreateSystemMessage(SystemMessage.MessageType.Stop);

            Thread.Sleep(500);

            Assert.That(supThread.IsAlive, Is.False);

            supThread.Abort();
            supThread.Join();
        }

        [Test]
        public void CanOverloadSupervisor()
        {
            Assert.That(_sender.Messages, Is.Empty);

            var supervisorOverload = new Supervisor(_receiver, _sender, () => new WorkerOverloadTest(), _supConfig);
            var supThread = new Thread(() => supervisorOverload.Start());
            supThread.Start();

            _receiver.CreateMessage();
            _receiver.CreateMessage();
            _receiver.CreateMessage();
            _receiver.CreateMessage();

            Thread.Sleep(100);

            Assert.That(_receiver.MessageQueue.Count, Is.EqualTo(2));

            supThread.Abort();
            supThread.Join();
        }

        [Test]
        public void CanChangeConfigurations()
        {
            Assert.That(_sender.Messages, Is.Empty);

            var supervisor = new Supervisor(_receiver, _sender, () => new WorkerTest(), _supConfig);
            var supThread = new Thread(() => supervisor.Start());
            supThread.Start();

            var random = new Random();
            var maxWorkers = random.Next(10, 50);
            var maxWaitTime = random.Next(10, 50);

            _receiver.CreateSystemMessage(SystemMessage.MessageType.ChangeConfigurationMaxWorkers, maxWorkers.ToString());
            _receiver.CreateSystemMessage(SystemMessage.MessageType.ChangeConfigurationWaitFreeThreadTime, maxWaitTime.ToString());
            _receiver.CreateSystemMessage(SystemMessage.MessageType.Stop);

            Thread.Sleep(500);

            Assert.That(supervisor.Configuration.MaxWorkers, Is.EqualTo(maxWorkers));
            Assert.That(supervisor.Configuration.WaitFreeThreadTime, Is.EqualTo(maxWaitTime));

            supThread.Abort();
            supThread.Join();
        }
    }
}
