namespace PetProjects.Framework.Kafka.Configurations.Consumer
{
    using System.Collections.Generic;

    public interface IConsumerConfiguration
    {
        int? PollTimeout { get; }

        Dictionary<string, object> GetConfigurations();
    }
}