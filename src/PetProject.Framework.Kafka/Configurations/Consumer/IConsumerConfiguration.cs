namespace PetProjects.Framework.Kafka.Configurations.Consumer
{
    using System.Collections.Generic;

    public interface IConsumerConfiguration
    {
        int MaxPollIntervalInMs { get; }

        bool AutoCommit { get; }

        int AutoCommitInterval { get; }

        Dictionary<string, object> GetConfigurations();
    }
}