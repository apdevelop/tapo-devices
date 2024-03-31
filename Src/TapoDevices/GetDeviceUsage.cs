using System.Text.Json.Serialization;

namespace TapoDevices
{
    public class GetDeviceUsage
    {
        public class Params
        {

        }

        public class Result
        {

        }

        public class ResultBulb : Result
        {
            [JsonPropertyName("time_usage")]
            public TimeUsage TimeUsage { get; set; }

            [JsonPropertyName("power_usage")]
            public TimeUsage PowerUsage { get; set; }

            [JsonPropertyName("saved_power")]
            public TimeUsage SavedPower { get; set; }
        }

        public class ResultPlug : Result
        {
            [JsonPropertyName("time_usage")]
            public TimeUsage TimeUsage { get; set; }
        }

        internal static TapoRequest<Params> CreateRequest() =>
            Utils.CreateTapoRequest<Params>("get_device_usage", null);
    }
}
