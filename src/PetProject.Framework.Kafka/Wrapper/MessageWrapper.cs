namespace PetProjects.Framework.Kafka.Wrapper
{
    public sealed class MessageWrapper
    {
        public string MessageType { get; set; }

        public object Message { get; set; }
    }
}