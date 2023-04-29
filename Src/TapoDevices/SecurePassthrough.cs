using System.Text.Json.Serialization;

namespace TapoDevices
{
    class SecurePassthrough
    {
        public class Params
        {
            [JsonPropertyName("request")]
            public string Request { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("response")]
            public string Response { get; set; }
        }

        internal static TapoRequest<Params> CreateRequest(Params parameters) =>
            Utils.CreateTapoRequest("securePassthrough", parameters);
    }
}
