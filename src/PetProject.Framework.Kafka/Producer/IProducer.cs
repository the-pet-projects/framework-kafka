namespace PetProject.Framework.Kafka.Producer
{
    using System.Threading.Tasks;
    using Topics;

    internal interface IProducer
    {
        Task ProduceAsync<TMessage>(TMessage message) where TMessage : IMessageContract;
    }
}