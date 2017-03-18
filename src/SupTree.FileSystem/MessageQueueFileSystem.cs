using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SupTree.Common;

namespace SupTree.FileSystem
{
    public class MessageQueueFileSystem : IMessageReceiver, IMessageSender
    {
        private readonly string _directoryPath;
        private readonly string _fileFilter;
        private readonly string _fileExtension;

        public MessageQueueFileSystem(string directoryPath, string fileFilter, string fileExtension)
        {
            _directoryPath = directoryPath;
            _fileFilter = fileFilter;
            _fileExtension = fileExtension;

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        public Message Receive()
        {
            string file;
            do
            {
                file = Directory.EnumerateFiles(_directoryPath, _fileFilter).FirstOrDefault(); 
            } while (string.IsNullOrEmpty(file));

            var fileContent = File.ReadAllBytes(file);
            var unzipedContent = Compression.UnGZip(fileContent, Encoding.UTF8);

            var message = JsonConvert.DeserializeObject<Message>(unzipedContent);

            File.Delete(file);

            return message;
        }

        public void Send(Message message)
        {
            var messageContent = JsonConvert.SerializeObject(message);
            if (messageContent == null)
                throw new ApplicationException("Could not serialize message");

            var messageGziped = Compression.GZip(messageContent, Encoding.UTF8);

            var fileName = Path.Combine(_directoryPath, string.Concat(Guid.NewGuid(), ".", _fileExtension));

            File.WriteAllBytes(fileName, messageGziped);
        }
    }
}
