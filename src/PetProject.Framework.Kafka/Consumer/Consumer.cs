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
    using PetProjects.Framework.Kafka.Exceptions;
    using PetProjects.Framework.Kafka.Logging;
    using PetProjects.Framework.Kafka.Serializer;
    using PetProjects.Framework.Kafka.Wrapper;
    using Utilities;

    public abstract class Consumer<TBaseMessage> : IConsumer<TBaseMessage>
        where TBaseMessage : IMessage
    {
        private readonly GenericKafkaLog logger;
        private readonly bool allowRetries;
        private readonly IConsumerConfiguration configuration;
        private readonly CancellationTokenSource tokenSource;
        private readonly Dictionary<Type, Action<object>> messageHandlers;
        private readonly ITopic<TBaseMessage> topic;

        private Consumer<string, MessageWrapper> confluentConsumer;

        private Task consumerTask;

        protected Consumer(ITopic<TBaseMessage> topic, IConsumerConfiguration configuration, ILogger logger, bool allowRetries = false)
        {
            this.logger = new GenericKafkaLog(logger);
            this.allowRetries = allowRetries;
            this.messageHandlers = new Dictionary<Type, Action<object>>();
            this.tokenSource = new CancellationTokenSource();
            this.configuration = configuration;
            this.topic = topic;
        }

        /// <inheritdoc />
        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// </summary>
        public void StartConsuming()
        {
            this.consumerTask = ConsumerTaskUtilities.StartLongRunningConsumer(() =>
            {
                this.confluentConsumer = new Consumer<string, MessageWrapper>(
                    this.configuration.GetConfigurations(),
                    new StringDeserializer(Encoding.UTF8),
                    new JsonDeserializer<MessageWrapper>());

                this.logger.KafkaLogWarning("Consumer is Starting.");

                this.confluentConsumer.OnMessage += this.HandleMessage;

                this.confluentConsumer.OnConsumeError += this.HandleOnConsumerError;

                this.confluentConsumer.OnError += this.HandleError;

                this.confluentConsumer.OnLog += this.HandleLogs;

                this.confluentConsumer.OnStatistics += this.HandleStatistics;

                this.confluentConsumer.Subscribe(this.topic.Name);

                while (this.tokenSource != null && !this.tokenSource.IsCancellationRequested)
                {
                    this.confluentConsumer.Poll(this.configuration.MaxPollIntervalInMs);
                }

                this.logger.KafkaLogWarning("Consumer is Stopped.");
            });
        }

        /// <inheritdoc />
        /// <summary>
        /// Handler for each message to be consumed inside the topic. If handler doesn't throw exception, message will be commited.
        /// </summary>
        public void Receive<TMessage>(Action<TMessage> handler)
        {
            this.messageHandlers[typeof(TMessage)] = msg => handler((TMessage)msg);
        }

        /// <inheritdoc />
        /// <summary>
        /// Handler for each message to be consumed inside the topic.
        /// Return type of handler should be a boolean that will be used to determine if message is to be commited or not.
        /// </summary>
        public void TryReceive<TMessage>(Func<TMessage, bool> handler)
        {
            this.messageHandlers[typeof(TMessage)] = msg =>
            {
                var result = handler((TMessage)msg);

                if (!result)
                {
                    throw new InternalConsumerException();
                }
            };
        }

        public void TryReceiveAsync<TMessage>(Func<TMessage, Task<bool>> handler)
        {
            this.messageHandlers[typeof(TMessage)] = async msg =>
             {
                 var result = await handler((TMessage)msg);

                 if (!result)
                 {
                     throw new InternalConsumerException();
                 }
             };
        }

        public void Dispose()
        {
            if (this.consumerTask != null)
            {
                this.logger.KafkaLogWarning("Consumer is Stopping.");
                this.tokenSource.Cancel();

                this.consumerTask.Wait();
                this.consumerTask.Dispose();

                if (this.confluentConsumer == null)
                {
                    return;
                }

                this.confluentConsumer.Dispose();
                this.confluentConsumer = null;
                this.logger.KafkaLogWarning("Consumer is Disposed.");
            }

            this.tokenSource?.Dispose();
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

        protected void RequeueMessageOnError(Message message)
        {
            var key = new StringDeserializer(Encoding.UTF8).Deserialize(message.Topic, message.Key);
            var messageContent = new JsonDeserializer<MessageWrapper>().Deserialize(message.Topic, message.Value);

            this.RequeueMessageOnError(message.Topic, message.TopicPartition, message.Offset, key, messageContent);
        }

        // It's ugly I know, I will beautify the shit out this later. for now it does the work.
        protected virtual void RequeueMessageOnError(string topicName, TopicPartition topicPartition, Offset offset, string key, MessageWrapper messageValue)
        {
            var bootstrapServers = this.configuration.GetConfigurations()["bootstrap.servers"] as string;

            using (var producer = new Producer<string, MessageWrapper>(new ProducerConfiguration($"retry-producer-{Guid.NewGuid()}", bootstrapServers).GetConfigurations(), new StringSerializer(Encoding.UTF8), new JsonSerializer<MessageWrapper>()))
            {
                var report = producer.ProduceAsync(topicName, key, messageValue).Result;

                if (!report.Error.HasError)
                {
                    // move offset forward to this message's offset + 1
                    this.confluentConsumer.CommitAsync(new[]
                    {
                        new TopicPartitionOffset(topicPartition, offset + 1L)
                    }).Wait();
                    return;
                }

                this.logger.KafkaLogCritical("Critical Error when retrying to send message. Topic: {topic} | Message: {message} | Timestamp: {timestamp} | Error: {error}", topicName, messageValue, report.Timestamp, report.Error);
            }
        }

        protected void RequeueMessageOnError(Message<string, MessageWrapper> message)
        {
            this.RequeueMessageOnError(message.Topic, message.TopicPartition, message.Offset, message.Key, message.Value);
        }

        /// <summary>
        /// May be overriden by the client in order to consumer messages and add custom treatment.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="consumerMessage">Message envelope with key value pair content.</param>
        protected void HandleMessage(object sender, Message<string, MessageWrapper> consumerMessage)
        {
            try
            {
                if (consumerMessage.Value == null)
                {
                    this.logger.KafkaLogInfo("ConsumerMessage has no content: {consumerMessage}", consumerMessage);
                }
                else
                {
                    var wrappedMessage = consumerMessage.Value;
                    this.CallHandler(wrappedMessage);
                }

                this.confluentConsumer.CommitAsync(consumerMessage).Wait();
            }
            catch (InternalConsumerException)
            {
                this.logger.KafkaLogWarning("Handler returned false while handling message {message}", consumerMessage);
                this.RequeueMessageOnError(consumerMessage);
            }
            catch (Exception ex)
            {
                this.logger.KafkaLogError("Exception occured while handling message {message}: {exception}", consumerMessage, ex);
                this.RequeueMessageOnError(consumerMessage);
            }
        }

        protected void CallHandler(MessageWrapper wrappedMessage)
        {
            var type = Type.GetType(wrappedMessage.MessageType);

            if (!this.messageHandlers.ContainsKey(type))
            {
                this.logger.KafkaLogError("An handler for this {type} of message does not exist. Please define one with Receive<> method!", type);
                return;
            }

            this.messageHandlers[type](wrappedMessage.Message);
        }
    }
}