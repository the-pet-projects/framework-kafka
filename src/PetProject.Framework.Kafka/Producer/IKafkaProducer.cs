namespace PetProject.Framework.Kafka.Producer
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Topics;

    internal interface IKafkaProducer<TTopic, TMessage> : IDisposable 
        where TMessage : IMessageContract
        where TTopic : ITopicContract
    {
        Task Produce(TMessage message);

        Task<Message<string, TMessage>> ProduceAsync(TMessage message);
    }
}