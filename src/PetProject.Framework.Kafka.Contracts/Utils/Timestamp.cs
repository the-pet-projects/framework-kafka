namespace PetProjects.Framework.Kafka.Contracts.Utils
{
    using System;

    /// <summary>
    /// Timestamp class to allow the usage of an Epoch timestamp in the contracts.
    /// </summary>
    public class Timestamp
    {
        public static readonly DateTime UnixTimeEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Creates a timestamp based in DateTime.UtcNow minus the UnixTimeEpoch (1970, 1, 1)
        /// </summary>
        public Timestamp()
        {
            this.UnixTimeEpochTimestamp = CalculateMicroseconds();
        }

        public long UnixTimeEpochTimestamp { get; set; }

        private static long CalculateMicroseconds()
        {
            return (DateTime.UtcNow - UnixTimeEpoch).Ticks / (TimeSpan.TicksPerMillisecond / 1000);
        }
    }
}