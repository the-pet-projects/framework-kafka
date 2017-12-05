namespace PetProjects.Framework.Kafka.Serializer
{
    using System.Collections.Generic;
    using System.Text;
    using Confluent.Kafka.Serialization;
    using Newtonsoft.Json;

    internal class JsonDeserializer<T> : IDeserializer<T>
    {
        private readonly IDeserializer<string> stringDeserializer;

        public JsonDeserializer()
        {
            this.stringDeserializer = new StringDeserializer(Encoding.UTF8);
        }

        public T Deserialize(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        public T Deserialize(string topic, byte[] data)
        {
            var str = this.stringDeserializer.Deserialize(topic, data);
            return JsonConvert.DeserializeObject<T>(str);
        }

        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }
    }
}