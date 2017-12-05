namespace PetProjects.Framework.Kafka.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Configurations.Consumer;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Contracts.Topics;
    using Microsoft.Extensions.Logging;
    using Serializer;
    using Wrapper;

    public abstract class Consumer<TBaseMessage> : IConsumer<TBaseMessage>
        where TBaseMessage : IMessage
    {
        private readonly ILogger logger;
        private readonly IConsumerConfiguration configuration;
        private readonly CancellationTokenSource tokenSource;
        private readonly Dictionary<Type, Delegate> messageHandlers;
        private readonly ITopic<TBaseMessage> topic;

        private Consumer<string, MessageWrapper> confluentConsumer;

        protected Consumer(ITopic<TBaseMessage> topic, IConsumerConfiguration configuration, ILogger logger)
        {
            this.logger = logger;
            this.messageHandlers = new Dictionary<Type, Delegate>();
            this.tokenSource = new CancellationTokenSource();
            this.configuration = configuration;
            this.topic = topic;
        }

        /// <inheritdoc />
        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// </summary>
        public bool StartConsuming()
        {
            this.confluentConsumer = new Consumer<string, MessageWrapper>(
                this.configuration.GetConfigurations(),
                new StringDeserializer(Encoding.UTF8),
                new JsonDeserializer<MessageWrapper>());

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
        public async Task<CommittedOffsets> CommitAsync()
        {
            return await this.confluentConsumer.CommitAsync();
        }

        /// <summary>
        /// Method to add custom treatment to Consumer Statistics.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="statistics">Statistics returned from Confluent Consumer.</param>
        protected virtual void HandleStatistics(object sender, string statistics)
        {
            this.logger.LogInformation("Framework Kafka Statistics: {statistics}", statistics);
        }

        /// <summary>
        /// Method to add custom treatment to Consumer Logs.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="logMessage">Log Message object returned from Confluent Consumer.</param>
        protected virtual void HandleLogs(object sender, LogMessage logMessage)
        {
            this.logger.LogInformation("Framework Kafka LogMessage: {logMessage}", logMessage);
        }

        /// <summary>
        /// Method to add custom treatment to Consumer Errors.
        /// </summary>
        /// <param name="sender">Message sender</param>
        /// <param name="error">Error object returned from Confluent Consumer</param>
        protected virtual void HandleError(object sender, Error error)
        {
            this.logger.LogError("Framework Kafka Error: {error}", error);
        }

        /// <summary>
        /// Must be implemented by the client in order to add Errors while consuming.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="message">Message envelope with the content to consume.</param>
        protected virtual void HandleOnConsumerError(object sender, Message message)
        {
            this.logger.LogWarning("Framework Kafka ConsumerError: {message}", message);
            this.RequeueMessageOnError(message);
        }

        protected abstract void RequeueMessageOnError(Message message);

        /// <summary>
        /// Must be implemented by the client in order to consumer messages and add custom treatment.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="consumerMessage">Message envelope with key value pair content.</param>
        protected void HandleMessage(object sender, Message<string, MessageWrapper> consumerMessage)
        {
            if (consumerMessage.Value == null)
            {
                this.logger.LogInformation("Framework Kafka ConsumerMessage has no content: {consumerMessage}", consumerMessage);
                return;
            }

            var wrappedMessage = consumerMessage.Value;
            var type = Type.GetType(wrappedMessage.MessageType);

            if (!this.messageHandlers.ContainsKey(type))
            {
                this.logger.LogError("An handler for this type of message does not exist. Please define one with ConsumerHandlerFor<> method!");
                throw new Exception("An handler for this type of message does not exist. Please define one with ConsumerHandlerFor<> method!");
            }

            this.messageHandlers[type].DynamicInvoke(wrappedMessage.Message);
        }
    }
}