using System.Text.Json.Serialization;

namespace TapoDevices
{
    class AddCountdownRule
    {
        public class Params
        {
            /// <summary>
            /// Is rule enabled.
            /// </summary>
            [JsonPropertyName("enable")]
            public bool Enable { get; set; }

            /// <summary>
            /// Delay before changing state, in seconds.
            /// </summary>
            [JsonPropertyName("delay")]
            public int Delay { get; set; }

            [JsonPropertyName("desired_states")]
            public ParamsStates DesiredStates { get; set; }
        }

        public class ParamsStates
        {
            [JsonPropertyName("on")]
            public bool On { get; set; }
        }

        public class Result
        {

        }

        internal static TapoRequest<Params> CreateRequest(Params parameters) =>
            Utils.CreateTapoRequest("add_countdown_rule", parameters);
    }
}
