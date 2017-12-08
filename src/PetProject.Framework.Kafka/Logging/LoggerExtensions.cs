namespace PetProjects.Framework.Kafka.Logging
{
    using Microsoft.Extensions.Logging;

    public static class LoggerExtensions
    {
        public static void KafkaLogWarning(this ILogger logger, string message, params object[] args)
        {
            using (logger.BeginScope("LogType: {logType}", LogConstants.LogTypes.Kafka))
            {
                logger.LogWarning(message, args);
            }
        }

        public static void KafkaLogInfo(this ILogger logger, string message, params object[] args)
        {
            using (logger.BeginScope("LogType: {logType}", LogConstants.LogTypes.Kafka))
            {
                logger.LogInformation(message, args);
            }
        }

        public static void KafkaLogCritical(this ILogger logger, string message, params object[] args)
        {
            using (logger.BeginScope("LogType: {logType}", LogConstants.LogTypes.Kafka))
            {
                logger.LogCritical(message, args);
            }
        }

        public static void KafkaLogError(this ILogger logger, string message, params object[] args)
        {
            using (logger.BeginScope("LogType: {logType}", LogConstants.LogTypes.Kafka))
            {
                logger.LogError(message, args);
            }
        }

        public static void KafkaLogTrace(this ILogger logger, string message, params object[] args)
        {
            using (logger.BeginScope("LogType: {logType}", LogConstants.LogTypes.Kafka))
            {
                logger.LogTrace(message, args);
            }
        }
    }
}