namespace PetProject.Framework.Kafka.Topics
{
    public static class MessageWrapperFactory<TBaseMessage>
    {
        public static MessageWrapper<TBaseMessage> CreateTyped<TMessage>(TMessage message)
        {
            return new MessageWrapper<TBaseMessage>
            {
                MessageType = typeof(TMessage).AssemblyQualifiedName,
                Message = message
            };
        }

        public static MessageWrapper CreateNonTyped<TMessage>(TMessage message)
        {
            return new MessageWrapper
            {
                MessageType = typeof(TMessage).AssemblyQualifiedName,
                Message = message
            };
        }
    }
}