using System.Text.Json.Serialization;

namespace TapoDevices
{
    class LoginDevice
    {
        public class Params
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("token")]
            public string Token { get; set; }
        }

        internal static TapoRequest<Params> CreateRequest(Params parameters) =>
            Utils.CreateTapoRequest("login_device", parameters);
    }
}
