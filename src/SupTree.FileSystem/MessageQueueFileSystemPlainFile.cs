using System.IO;

namespace SupTree.FileSystem
{
    public class MessageQueueFileSystemPlainFile : MessageQueueFileSystem
    {
        public MessageQueueFileSystemPlainFile(string directoryPath, string fileFilter, string fileExtension,
            int waitForNewFileTime = 1000) : base(directoryPath, fileFilter, fileExtension, waitForNewFileTime)
        {
        }

        protected override Message GetMessageFromFile(string file)
        {
            var fileContent = File.ReadAllText(file);
            
            return new Message
            {
                Body = fileContent,
                Format = "TEXT"
            };
        }

        protected override void SetFileContent(string fileName, Message message)
        {
            File.WriteAllText(fileName, message.Body);
        }
    }
}
