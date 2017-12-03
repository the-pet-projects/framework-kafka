namespace PetProjects.Framework.Kafka.Producer
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Contracts.Topics;
    using Wrapper;

    public interface IProducer<TBaseMessage> : IDisposable
        where TBaseMessage : IMessage
    {
        Task Produce<TMessage>(TMessage message) where TMessage : IMessage;

        Task<Message<string, MessageWrapper<TBaseMessage>>> ProduceAsync<TMessage>(TMessage message) where TMessage : IMessage;
    }
}