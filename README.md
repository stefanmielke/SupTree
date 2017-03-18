# SupTree
Simple worker processing through message passing.

# Usage

- Reference SupTree, Suptree.Common and the modules you want to use for messaging (such as MSMQ or FileSystem);
- Implement a Worker, inheriting from SupTree.Worker and implement the methods you want to use;
- Create a supervisor passing the required attributes.

## Example

    // receive messages from the FileSystem
    var receiver = new MessageQueueFileSystem(@"C:\test_folder", "*.json", "json");
    
    // send messages to MSMQ
    var sender = new new MessageQueueMSMQ("test_queue");

    // configure to process at most 2 messages at any time
    var supConfig = new SupervisorConfiguration
    {
        MaxWorkers = 2
    };

    // create a supervisor using the configuration above, using 'WorkerTest' to process messages
    var supervisor = new Supervisor(receiver, sender, () => new WorkerTest(), supConfig);
    
    // start the supervisor and wait for it to exit
    supervisor.Start();
