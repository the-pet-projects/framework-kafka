namespace PetProject.Framework.Kafka.Topics
{
    public sealed class MessageWrapper<T>
    {
        public string MessageType { get; set; }

        public object Message { get; set; }
    }

    public sealed class MessageWrapper
    {
        public string MessageType { get; set; }

        public object Message { get; set; }
    }
}