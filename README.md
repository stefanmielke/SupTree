# SupTree
Simple worker processing through message passing.

# Usage

- Reference SupTree, Suptree.Common and the modules you want to use for messaging (such as MSMQ or FileSystem);
- Implement a Worker, inheriting from SupTree.Worker and implement the methods you want to use;
- Create a supervisor passing the required attributes.

## Examples

### Basic Setup

```csharp
// receive messages from the FileSystem
var receiver = new MessageQueueFileSystem(@"C:\test_folder", "*.json", "json");

// send messages to MSMQ
var sender = new MessageQueueMSMQ("test_queue");

var container = new StandardKernel();
container.Bind<IMessageReceiver>().ToConstant(receiver); // binding receiver (required)
container.Bind<IWorker>().To<WorkerTest>(); // binding worker (required)

container.Bind<IMessageSender>().ToConstant(sender); // binding default sender (optional, to ease the usage later)

// configure to process at most 2 messages at any time
var supConfig = new SupervisorConfiguration
{
    MaxWorkers = 2
};

// create a supervisor using the configuration above
var supervisor = new Supervisor(container, supConfig);

// start the supervisor and wait for it to exit
supervisor.Start();
```

### Sending Messages in Worker

```csharp
var message = new Message();
message.SetBody(new SimpleMessageObject { Guid = new Guid().ToString() });

// get the correct sender from the supervisor (configured earlier)
var sender = Supervisor.Container.Get<IMessageSender>();
sender.Send(message);
```

### Receiving Messages

Messages are automatically received using the IMessageReceiver passed to the Supervisor and sent to the respective workers.
