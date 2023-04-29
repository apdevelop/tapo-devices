using System.Text.Json.Serialization;

namespace TapoDevices
{
    class TapoResponse<TResult>
    {
        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; } // TODO: enum with error codes

        [JsonPropertyName("result")]
        public TResult Result { get; set; }
    }
}
