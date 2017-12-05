namespace PetProjects.Framework.Kafka.Serializer
{
    using System.Collections.Generic;
    using System.Text;
    using Confluent.Kafka.Serialization;
    using Newtonsoft.Json;

    internal class JsonSerializer<T> : ISerializer<T>
    {
        private readonly ISerializer<string> stringSerializer;

        private readonly JsonSerializerSettings settings;

        public JsonSerializer()
        {
            this.stringSerializer = new StringSerializer(Encoding.UTF8);
            this.settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public byte[] Serialize(string topic, T data)
        {
            return this.stringSerializer.Serialize(topic, JsonConvert.SerializeObject((object)data, this.settings));
        }

        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }
    }
}