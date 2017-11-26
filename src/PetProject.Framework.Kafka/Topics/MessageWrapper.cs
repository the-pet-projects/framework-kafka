namespace PetProject.Framework.Kafka.Topics
{
    public class MessageWrapper<T>
    {
        public string MessageType { get; set; }

        public object Message { get; set; }
    }

    public class MessageWrapper
    {
        public string MessageType { get; set; }

        public object Message { get; set; }
    }
}