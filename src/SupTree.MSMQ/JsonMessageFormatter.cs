using System;
using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;
using SupTree.Common;

namespace SupTree.MSMQ
{
    internal class JsonMessageFormatter : IMessageFormatter
    {
        private static readonly JsonSerializerSettings DefaultSerializerSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

        private readonly JsonSerializerSettings _serializerSettings;

        public Encoding Encoding { get; }

        public JsonMessageFormatter(Encoding encoding = null)
            : this(encoding, null)
        {
        }

        private JsonMessageFormatter(Encoding encoding, JsonSerializerSettings serializerSettings = null)
        {
            Encoding = encoding ?? Encoding.UTF8;
            _serializerSettings = serializerSettings ?? DefaultSerializerSettings;
        }

        public bool CanRead(System.Messaging.Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var stream = message.BodyStream;

            return stream != null
                && stream.CanRead
                && stream.Length > 0;
        }

        public object Clone()
        {
            return new JsonMessageFormatter(Encoding, _serializerSettings);
        }

        public object Read(System.Messaging.Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (CanRead(message) == false)
                return null;

            using (var reader = new StreamReader(message.BodyStream, Encoding))
            {
                using (var memoryReader = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memoryReader);
                    var bytes = memoryReader.ToArray();
                    var json = Compression.UnGZip(bytes, Encoding);

                    return JsonConvert.DeserializeObject(json, _serializerSettings);
                }
            }
        }

        public void Write(System.Messaging.Message message, object obj)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            string json = JsonConvert.SerializeObject(obj, Formatting.None, _serializerSettings);
            var compressedJson = Compression.GZip(json, Encoding);

            message.BodyStream = new MemoryStream(compressedJson);

            //Need to reset the body type, in case the same message
            //is reused by some other formatter.
            message.BodyType = 0;
        }
    }
}