namespace SupTree
{
    public class SystemMessage
    {
        public MessageType Type { get; set; }

        public string Value { get; set; }

        public enum MessageType
        {
            Stop,
            ChangeConfigurationMaxWorkers,
            ChangeConfigurationWaitFreeThreadTime
        }
    }
}
