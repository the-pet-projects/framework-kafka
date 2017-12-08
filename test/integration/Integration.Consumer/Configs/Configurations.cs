namespace Integration.Consumer.Configs
{
    using System;
    using System.IO;
    using Contracts;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using PetProjects.Framework.Consul;
    using PetProjects.Framework.Consul.Store;
    using PetProjects.Framework.Kafka.Configurations.Consumer;
    using PetProjects.Framework.Kafka.Consumer;

    public class Configurations
    {
        public Configurations()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Configs/appsettings.json", optional: false, reloadOnChange: false);

            this.Configuration = configurationBuilder.Build();

            this.ServiceCollection.AddPetProjectConsulServices(this.Configuration, true);
            this.ServiceCollection.AddSingleton<ILogger>(NullLogger.Instance);

            IStringKeyValueStore configStore;

            using (var tempProvider = this.ServiceCollection.BuildServiceProvider())
            {
                configStore = tempProvider.GetRequiredService<IStringKeyValueStore>();
            }

            var pollInterval = configStore.GetAndConvertValue<int>("kafka/consumer/pollInterval");
            var brokers = configStore.GetAndConvertValue<string>("kafka/brokersList").Split(',');
            var groupId = configStore.GetAndConvertValue<string>("kafka/consumer/consumerGroupId");
            var clientIdPrefix = configStore.GetAndConvertValue<string>("kafka/consumer/clientIdPrefix");

            this.ServiceCollection.AddSingleton<IConsumerConfiguration>(
                new ConsumerConfiguration(
                        groupId,
                        $"{clientIdPrefix}-{Guid.NewGuid()}",
                        brokers)
                    .SetPollIntervalInMs(pollInterval));

            this.ServiceCollection.AddSingleton<IConsumer<ItemCommandsV1>, TestConsumer>();

            this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();
        }

        public IServiceCollection ServiceCollection { get; set; } = new ServiceCollection();

        public IConfigurationRoot Configuration { get; private set; }

        public ServiceProvider ServiceProvider { get; set; }
    }
}