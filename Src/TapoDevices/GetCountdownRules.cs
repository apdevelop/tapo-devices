using System.Text.Json.Serialization;

namespace TapoDevices
{
    public class GetCountdownRules
    {
        public class Params
        {

        }

        public class ResultStates
        {
            [JsonPropertyName("on")]
            public bool On { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("enable")]
            public bool Enable { get; set; }

            [JsonPropertyName("countdown_rule_max_count")]
            public int RulesMaxCount { get; set; }

            [JsonPropertyName("rule_list")]
            public ResultRule[] Rules { get; set; }
        }

        public class ResultRule
        {
            [JsonPropertyName("enable")]
            public bool Enable { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            /// <summary>
            /// Initially set delay before changing state, in seconds.
            /// </summary>
            [JsonPropertyName("delay")]
            public int Delay { get; set; }

            /// <summary>
            /// Currently remaining time before changing state, in seconds.
            /// </summary>
            [JsonPropertyName("remain")]
            public int Remain { get; set; }

            [JsonPropertyName("desired_states")]
            public ResultStates DesiredStates { get; set; }
        }

        internal static TapoRequest<Params> CreateRequest() =>
            Utils.CreateTapoRequest<Params>("get_countdown_rules", null);
    }
}
