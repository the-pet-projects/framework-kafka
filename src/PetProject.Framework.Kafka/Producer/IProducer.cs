namespace PetProjects.Framework.Kafka.Producer
{
    using System;
    using System.Threading.Tasks;

    using Confluent.Kafka;

    using PetProjects.Framework.Kafka.Contracts.Topics;
    using PetProjects.Framework.Kafka.Wrapper;

    public interface IProducer<TBaseMessage> : IDisposable
        where TBaseMessage : IMessage
    {
        void Produce<TMessage>(TMessage message, IDeliveryHandler<string, MessageWrapper> deliveryHandler = null) where TMessage : IMessage;

        Task<Message<string, MessageWrapper>> ProduceAsync<TMessage>(TMessage message) where TMessage : IMessage;
    }
}