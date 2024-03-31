using System.Text.Json.Serialization;

namespace TapoDevices
{
    public class GetLedInfo
    {
        public class Params
        {

        }

        public class Result
        {

        }

        public class ResultPlug : Result
        {
            /// <summary>
            /// Current status of LED (on/off).
            /// </summary>
            [JsonPropertyName("led_status")]
            public bool LedStatus { get; set; }

            /// <summary>
            /// LED rule ("always", "never", "night_mode").
            /// </summary>
            [JsonPropertyName("led_rule")]
            public string LedRule { get; set; }

            [JsonPropertyName("night_mode")]
            public NightMode NightMode { get; set; }
        }

        public class NightMode
        {
            /// <summary>
            /// Night mode type ("sunrise_sunset", "custom").
            /// </summary>
            [JsonPropertyName("night_mode_type")]
            public string NightModeType { get; set; }

            /// <summary>
            /// Start time, in minutes from day start.
            /// </summary>
            [JsonPropertyName("start_time")]
            public int StartTime { get; set; }

            /// <summary>
            /// End time, in minutes from day start.
            /// </summary>
            [JsonPropertyName("end_time")]
            public int EndTime { get; set; }

            [JsonPropertyName("sunrise_offset")]
            public int SunriseOffset { get; set; }

            [JsonPropertyName("sunset_offset")]
            public int SunsetOffset { get; set; }
        }

        internal static TapoRequest<Params> CreateRequest() =>
            Utils.CreateTapoRequest<Params>("get_led_info", null);
    }
}
