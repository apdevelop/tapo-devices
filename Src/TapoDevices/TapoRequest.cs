using System.Text.Json.Serialization;

namespace TapoDevices
{
    class TapoRequest<TParams>
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("params")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TParams Parameters { get; set; }

        [JsonPropertyName("requestTimeMils")]
        public long RequestTimeMilliseconds { get; set; }
    }
}
