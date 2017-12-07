namespace PetProjects.Framework.Kafka.Logging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Wrapper to segregate Kafka logs in kibana.
    /// </summary>
    public class GenericKafkaLog
    {
        private readonly ILogger logger;

        public GenericKafkaLog(ILogger logger)
        {
            this.logger = logger;
        }

        public void KafkaLogWarning(string message, params object[] args)
        {
            this.logger.LogWarning(LogConstants.LogMessages.OriginMessage + message, LogConstants.LogTypes.Kafka, args);
        }

        public void KafkaLogInfo(string message, params object[] args)
        {
            this.logger.LogInformation(LogConstants.LogMessages.OriginMessage + message, LogConstants.LogTypes.Kafka, args);
        }

        public void KafkaLogCritical(string message, params object[] args)
        {
            this.logger.LogCritical(LogConstants.LogMessages.OriginMessage + message, LogConstants.LogTypes.Kafka, args);
        }

        public void KafkaLogError(string message, params object[] args)
        {
            this.logger.LogError(LogConstants.LogMessages.OriginMessage + message, LogConstants.LogTypes.Kafka, args);
        }
    }
}