namespace PetProject.Framework.Kafka.Configurations.Consumer
{
    using System.Collections.Generic;
    using Topics;

    public interface IConsumerConfiguration
    {
        ITopicContract Topic { get; }

        int? PollTimeout { get; }

        Dictionary<string, object> GetConfigurations();
    }
}