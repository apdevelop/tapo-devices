using System.Text.Json.Serialization;

namespace TapoDevices
{
    class Handshake
    {
        public class Params
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }
        }

        internal static TapoRequest<Params> CreateRequest(Params parameters) =>
            Utils.CreateTapoRequest("handshake", parameters);
    }
}
