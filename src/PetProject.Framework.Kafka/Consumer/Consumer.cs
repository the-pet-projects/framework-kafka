namespace PetProject.Framework.Kafka.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Configurations.Consumer;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Contracts.Topics;
    using Newtonsoft.Json;
    using Wrapper;

    public abstract class Consumer<TBaseMessage> : IConsumer<TBaseMessage>
        where TBaseMessage : IMessage
    {
        private readonly IConsumerConfiguration configuration;
        private readonly CancellationTokenSource tokenSource;
        private readonly Dictionary<Type, Delegate> messageHandlers;
        private readonly JsonSerializerSettings settings;
        private readonly ITopic<TBaseMessage> topic;

        private Consumer<string, string> confluentConsumer;

        protected Consumer(ITopic<TBaseMessage> topic, IConsumerConfiguration configuration)
        {
            this.messageHandlers = new Dictionary<Type, Delegate>();
            this.tokenSource = new CancellationTokenSource();
            this.configuration = configuration;
            this.topic = topic;

            this.settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// Inititates a task in the backgroung with the LongRunning flag.
        /// </summary>
        public bool StartConsuming()
        {
            this.confluentConsumer = new Consumer<string, string>(
                this.configuration.GetConfigurations(),
                new StringDeserializer(Encoding.UTF8),
                new StringDeserializer(Encoding.UTF8));

            this.confluentConsumer.OnMessage += this.HandleMessage;

            this.confluentConsumer.OnConsumeError += this.HandleOnConsumerError;

            this.confluentConsumer.OnError += this.HandleError;

            this.confluentConsumer.OnLog += this.HandleLogs;

            this.confluentConsumer.OnStatistics += this.HandleStatistics;

            this.confluentConsumer.Subscribe(this.topic.TopicFullName);

            while (this.tokenSource != null && !this.tokenSource.IsCancellationRequested)
            {
                this.confluentConsumer.Poll(this.configuration.PollTimeout ?? 100);
            }

            return true;
        }

        public void ConsumerHandlerFor<TMessage>(Action<TMessage> handler)
        {
            this.messageHandlers[typeof(TMessage)] = handler;
        }

        public void Dispose()
        {
            this.tokenSource.Cancel();
            this.tokenSource?.Dispose();

            if (this.confluentConsumer == null)
            {
                return;
            }

            this.confluentConsumer.Dispose();
            this.confluentConsumer = null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Decorator around Confluent Consumer to commit messages asynchronously after success.
        /// </summary>
        public void CommitAsync()
        {
            this.confluentConsumer.CommitAsync();
        }

        /// <summary>
        /// Method to add custom treatment to Consumer Statistics.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="statistics">Statistics returned from Confluent Consumer.</param>
        protected abstract void HandleStatistics(object sender, string statistics);

        /// <summary>
        /// Method to add custom treatment to Consumer Logs.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="logMessage">Log Message object returned from Confluent Consumer.</param>
        protected abstract void HandleLogs(object sender, LogMessage logMessage);

        /// <summary>
        /// Method to add custom treatment to Consumer Errors.
        /// </summary>
        /// <param name="sender">Message sender</param>
        /// <param name="error">Error object returned from Confluent Consumer</param>
        protected abstract void HandleError(object sender, Error error);

        /// <summary>
        /// Must be implemented by the client in order to add Errors while consuming.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="message">Message envelope with the content to consume.</param>
        protected abstract void HandleOnConsumerError(object sender, Confluent.Kafka.Message message);

        /// <summary>
        /// Must be implemented by the client in order to consumer messages and add custom treatment.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="message">Message envelope with the content to consume.</param>
        protected void HandleMessage(object sender, Message<string, string> consumerMessage)
        {
            if (consumerMessage.Value == null)
            {
                return;
            }

            MessageWrapper wrappedMessage;
            try
            {
                wrappedMessage = JsonConvert.DeserializeObject<MessageWrapper>(consumerMessage.Value, this.settings);
            }
            catch (Exception)
            {
                throw new Exception("Consumer error when deserializing.");
            }

            var type = Type.GetType(wrappedMessage.MessageType);

            if (!this.messageHandlers.ContainsKey(type))
            {
                throw new Exception("An handler for this type of message does not exist. Please define one with ConsumerHandlerFor<> method!");
            }

            this.messageHandlers[type].DynamicInvoke(wrappedMessage.Message);
        }
    }
}