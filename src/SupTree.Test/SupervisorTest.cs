using System.Threading;
using NUnit.Framework;
using SupTree.Test.TestImplementations;

namespace SupTree.Test
{
    [TestFixture]
    internal class SupervisorTest
    {
        private readonly Supervisor _supervisor;
        private readonly Supervisor _supervisorFailure;
        private readonly Supervisor _supervisorError;
        private readonly Supervisor _supervisorOverload;
        private readonly ReceiverTest _receiver;
        private readonly SenderTest _sender;

        public SupervisorTest()
        {
            _receiver = new ReceiverTest();
            _sender = new SenderTest();

            var supConfig = new SupervisorConfiguration
            {
                MaxWorkers = 2,
                MaxWorkersOverload = 2
            };

            _supervisor = new Supervisor(_receiver, _sender, () => new WorkerTest(), supConfig);
            _supervisorFailure = new Supervisor(_receiver, _sender, () => new WorkerFailureTest(), supConfig);
            _supervisorError = new Supervisor(_receiver, _sender, () => new WorkerErrorTest(), supConfig);
            _supervisorOverload = new Supervisor(_receiver, _sender, () => new WorkerOverloadTest(), supConfig);
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

            var supThread = new Thread(() => _supervisor.Start());
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

            var supThread = new Thread(() => _supervisorFailure.Start());
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

            var supThread = new Thread(() => _supervisorError.Start());
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

            var supThread = new Thread(() => _supervisor.Start());
            supThread.Start();

            _receiver.CreateStopMessage();

            Thread.Sleep(500);

            Assert.That(supThread.IsAlive, Is.False);

            supThread.Abort();
            supThread.Join();
        }

        [Test]
        public void CanOverloadSupervisor()
        {
            Assert.That(_sender.Messages, Is.Empty);

            var supThread = new Thread(() => _supervisorOverload.Start());
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
    }
}
