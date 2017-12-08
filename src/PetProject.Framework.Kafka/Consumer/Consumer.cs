namespace PetProjects.Framework.Kafka.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;

    using Microsoft.Extensions.Logging;

    using PetProjects.Framework.Kafka.Configurations.Consumer;
    using PetProjects.Framework.Kafka.Configurations.Producer;
    using PetProjects.Framework.Kafka.Contracts.Topics;
    using PetProjects.Framework.Kafka.Logging;
    using PetProjects.Framework.Kafka.Serializer;
    using PetProjects.Framework.Kafka.Wrapper;

    public abstract class Consumer<TBaseMessage> : IConsumer<TBaseMessage>
        where TBaseMessage : IMessage
    {
        private readonly GenericKafkaLog logger;
        private readonly bool allowRetries;
        private readonly IConsumerConfiguration configuration;
        private readonly CancellationTokenSource tokenSource;
        private readonly Dictionary<Type, Delegate> messageHandlers;
        private readonly ITopic<TBaseMessage> topic;

        private Consumer<string, MessageWrapper> confluentConsumer;

        protected Consumer(ITopic<TBaseMessage> topic, IConsumerConfiguration configuration, ILogger logger, bool allowRetries = false)
        {
            this.logger = new GenericKafkaLog(logger);
            this.allowRetries = allowRetries;
            this.messageHandlers = new Dictionary<Type, Delegate>();
            this.tokenSource = new CancellationTokenSource();
            this.configuration = configuration;
            this.topic = topic;
        }

        /// <inheritdoc />
        /// <summary>
        /// Flag which signals if the consumer is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// </summary>
        public void StartConsuming()
        {
            this.confluentConsumer = new Consumer<string, MessageWrapper>(
                this.configuration.GetConfigurations(),
                new StringDeserializer(Encoding.UTF8),
                new JsonDeserializer<MessageWrapper>());

            this.IsRunning = true;

            this.confluentConsumer.OnMessage += this.HandleMessage;

            this.confluentConsumer.OnConsumeError += this.HandleOnConsumerError;

            this.confluentConsumer.OnError += this.HandleError;

            this.confluentConsumer.OnLog += this.HandleLogs;

            this.confluentConsumer.OnStatistics += this.HandleStatistics;

            this.confluentConsumer.Subscribe(this.topic.TopicFullName);

            while (this.tokenSource != null && !this.tokenSource.IsCancellationRequested && this.IsRunning)
            {
                this.confluentConsumer.Poll(this.configuration.MaxPollIntervalInMs);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Method to signal the consumer to stop.
        /// </summary>
        public void StopConsuming()
        {
            this.IsRunning = false;
            this.tokenSource.Cancel();
            this.Dispose();
        }

        /// <inheritdoc />
        /// <summary>
        /// Handler for each message to be consumed inside the topic.
        /// </summary>
        public void Receive<TMessage>(Action<TMessage> handler)
        {
            this.messageHandlers[typeof(TMessage)] = handler;
        }

        public void Dispose()
        {
            if (this.IsRunning)
            {
                return;
            }

            this.StopConsumer();
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
            this.logger.KafkaLogInfo("Statistics: {statistics}", statistics);
        }

        /// <summary>
        /// Method to add custom treatment to Consumer Logs.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="logMessage">Log Message object returned from Confluent Consumer.</param>
        protected virtual void HandleLogs(object sender, LogMessage logMessage)
        {
            this.logger.KafkaLogInfo("LogMessage: {logMessage}", logMessage);
        }

        /// <summary>
        /// Method to add custom treatment to Consumer Errors.
        /// </summary>
        /// <param name="sender">Message sender</param>
        /// <param name="error">Error object returned from Confluent Consumer</param>
        protected virtual void HandleError(object sender, Error error)
        {
            this.logger.KafkaLogError("Error: {error}", error);
        }

        /// <summary>
        /// May be overriden by the client in order to handle consumer internal errors when consuming a message.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="message">Message envelope with the content to consume.</param>
        protected virtual void HandleOnConsumerError(object sender, Message message)
        {
            this.logger.KafkaLogWarning("ConsumerError: {messageContent}", message);

            // Chain ErrorHandling for specific cases
            if (message.Error.IsLocalError)
            {
                if (message.Error.Code == ErrorCode.Local_ValueDeserialization)
                {
                    this.logger.KafkaLogWarning("Consumer Value Deserialization Error Code: {errorCode} | Reason: {errorMessage}", message.Error.Code, message.Error.Reason);
                }

                if (message.Error.Code == ErrorCode.BrokerNotAvailable)
                {
                    this.logger.KafkaLogCritical("Consumer Broker Error: {errorCode} | Reason: {errorMessage}", message.Error.Code, message.Error.Reason);
                }

                return;
            }

            if (this.allowRetries)
            {
                this.RequeueMessageOnError(message);
            }
        }

        // It's ugly I know, I will beautify the shit out this later. for now it does the work.
        protected virtual void RequeueMessageOnError(Message message)
        {
            var bootstrapServers = this.configuration.GetConfigurations()["bootstrap.servers"] as string;
            using (var producer = new Producer<string, MessageWrapper>(new ProducerConfiguration($"retry-producer-{Guid.NewGuid()}", bootstrapServers).GetConfigurations(), new StringSerializer(Encoding.UTF8), new JsonSerializer<MessageWrapper>()))
            {
                var key = new StringDeserializer(Encoding.UTF8).Deserialize(message.Topic, message.Key);
                var messageContent = new JsonDeserializer<MessageWrapper>().Deserialize(message.Topic, message.Value);

                var report = producer.ProduceAsync(message.Topic, key, messageContent).Result;

                if (!report.Error.HasError)
                {
                    return;
                }

                this.logger.KafkaLogCritical("Critical Error when retrying to send message. Topic: {topic} | Message: {message} | Timestamp: {timestamp} | Error: {error}", message.Topic, messageContent, report.Timestamp, report.Error);
            }
        }

        /// <summary>
        /// May be overriden by the client in order to consumer messages and add custom treatment.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="consumerMessage">Message envelope with key value pair content.</param>
        protected void HandleMessage(object sender, Message<string, MessageWrapper> consumerMessage)
        {
            if (consumerMessage.Value == null)
            {
                this.logger.KafkaLogInfo("ConsumerMessage has no content: {consumerMessage}", consumerMessage);
                return;
            }

            var wrappedMessage = consumerMessage.Value;

            this.CallHandler(wrappedMessage);
        }

        protected void CallHandler(MessageWrapper wrappedMessage)
        {
            var type = Type.GetType(wrappedMessage.MessageType);

            if (!this.messageHandlers.ContainsKey(type))
            {
                this.logger.KafkaLogError("An handler for this {type} of message does not exist. Please define one with Receive<> method!", type);
                return;
            }

            this.messageHandlers[type].DynamicInvoke(wrappedMessage.Message);
        }

        private void StopConsumer()
        {
            this.tokenSource?.Dispose();

            if (this.confluentConsumer == null)
            {
                return;
            }

            this.confluentConsumer.Dispose();
            this.confluentConsumer = null;
        }
    }
}