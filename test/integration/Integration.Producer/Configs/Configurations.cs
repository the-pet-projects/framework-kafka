namespace Integration.Producer.Configs
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
    using PetProjects.Framework.Kafka.Configurations.Producer;
    using PetProjects.Framework.Kafka.Producer;

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

            this.ServiceCollection.AddSingleton<IProducerConfiguration>(sp =>
            {
                var brokers = configStore.GetAndConvertValue<string>("kafka/brokersList").Split(',');
                var clientId = configStore.GetAndConvertValue<string>("kafka/producer/clientId");

                return new ProducerConfiguration($"{clientId}{Guid.NewGuid()}", brokers);
            });

            IProducerConfiguration pconfig;
            using (var tempProvider = this.ServiceCollection.BuildServiceProvider())
            {
                pconfig = tempProvider.GetRequiredService<IProducerConfiguration>();
            }

            this.ServiceCollection.AddSingleton<IProducer<ItemCommandsV1>>(new Producer<ItemCommandsV1>(new ItemCommandsTopic(), pconfig));

            this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();
        }

        public IServiceCollection ServiceCollection { get; set; } = new ServiceCollection();

        public IConfigurationRoot Configuration { get; private set; }

        public ServiceProvider ServiceProvider { get; set; }
    }
}