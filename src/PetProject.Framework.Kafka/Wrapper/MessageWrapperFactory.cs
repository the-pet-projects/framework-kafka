namespace PetProjects.Framework.Kafka.Wrapper
{
    public static class MessageWrapperFactory
    {
        public static MessageWrapper Create<TMessage>(TMessage message)
        {
            return new MessageWrapper
            {
                MessageType = typeof(TMessage).AssemblyQualifiedName,
                Message = message
            };
        }
    }
}