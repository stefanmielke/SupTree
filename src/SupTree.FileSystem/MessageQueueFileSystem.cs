using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using SupTree.Common;

namespace SupTree.FileSystem
{
    public interface IMessageQueueFileSystem : IMessageReceiver, IMessageSender { }

    public class MessageQueueFileSystem : IMessageQueueFileSystem
    {
        private readonly string _directoryPath;
        private readonly string _fileFilter;
        private readonly string _fileExtension;
        private readonly int _waitForNewFileTime;

        public MessageQueueFileSystem(string directoryPath, string fileFilter, string fileExtension, int waitForNewFileTime = 1000)
        {
            _directoryPath = directoryPath;
            _fileFilter = fileFilter;
            _fileExtension = fileExtension;
            _waitForNewFileTime = waitForNewFileTime;

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        public Message Receive()
        {
            string file;
            do
            {
                file = Directory.EnumerateFiles(_directoryPath, _fileFilter).FirstOrDefault(); 
                if (string.IsNullOrEmpty(file))
                    Thread.Sleep(_waitForNewFileTime);

            } while (string.IsNullOrEmpty(file));

            Message message;
            try
            {
                message = GetMessageFromFile(file);
            }
            catch
            {
                File.Move(file, string.Concat(file, ".error"));
                return null;
            }

            File.Delete(file);

            return message;
        }

        protected virtual Message GetMessageFromFile(string file)
        {
            var fileContent = File.ReadAllBytes(file);
            var unzipedContent = Compression.UnGZip(fileContent, Encoding.UTF8);

            return JsonConvert.DeserializeObject<Message>(unzipedContent);
        }

        public void Send(Message message)
        {
            var fileName = Path.Combine(_directoryPath, string.Concat(Guid.NewGuid(), ".", message.Tag ?? "ALL", ".", _fileExtension));

            SetFileContent(fileName, message);
        }

        protected virtual void SetFileContent(string fileName, Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            File.WriteAllBytes(fileName, messageGziped);
        }

        public void Dispose()
        {
        }
    }
}
