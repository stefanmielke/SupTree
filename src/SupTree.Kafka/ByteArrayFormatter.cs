using Confluent.Kafka.Serialization;

namespace SupTree.Kafka
{
    internal class ByteArrayFormatter : IDeserializer<byte[]>, ISerializer<byte[]>
    {
        public byte[] Deserialize(byte[] data)
        {
            return data;
        }

        public byte[] Serialize(byte[] data)
        {
            return data;
        }
    }
}