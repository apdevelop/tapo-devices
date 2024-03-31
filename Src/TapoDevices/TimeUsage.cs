using System.Text.Json.Serialization;

namespace TapoDevices
{
    public class TimeUsage
    {
        /// <summary>
        /// Today.
        /// </summary>
        [JsonPropertyName("today")]
        public int Today { get; set; }

        /// <summary>
        /// Past 7 days.
        /// </summary>
        [JsonPropertyName("past7")]
        public int Past7 { get; set; }

        /// <summary>
        /// Past 30 days.
        /// </summary>
        [JsonPropertyName("past30")]
        public int Past30 { get; set; }
    }
}
