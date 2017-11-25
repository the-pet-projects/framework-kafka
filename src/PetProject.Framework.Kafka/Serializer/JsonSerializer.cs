namespace PetProject.Framework.Kafka.Serializer
{
    using System.Text;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Newtonsoft.Json;

    internal class JsonSerializer<T> : ISerializer<T>
    {
        private readonly ISerializer<string> stringSerializer;

        public JsonSerializer()
        {
            this.stringSerializer = new StringSerializer(Encoding.UTF8);
        }

        public byte[] Serialize(T data)
        {
            return this.stringSerializer.Serialize(JsonConvert.SerializeObject((object)data));
        }

        public T Deserialize(Message<Null, string> message)
        {
            return JsonConvert.DeserializeObject<T>(message.Value);
        }
    }
}