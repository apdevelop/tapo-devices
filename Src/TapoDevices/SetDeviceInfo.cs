using System.Text.Json.Serialization;

namespace TapoDevices
{
    public class SetDeviceInfo
    {
        public class Params
        {
            [JsonPropertyName("device_on")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public bool? DeviceOn { get; set; }
        }

        public class ParamsBulb : Params
        {
            /// <summary>
            /// Brightness.
            /// </summary>
            /// <remarks>
            /// Range 1..100.
            /// </remarks>
            [JsonPropertyName("brightness")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Brightness { get; set; }

            /// <summary>
            /// Hue.
            /// </summary>
            /// <remarks>Range 0..359.</remarks>
            [JsonPropertyName("hue")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Hue { get; set; }

            /// <summary>
            /// Saturation.
            /// </summary>
            /// <remarks>Range 0..100.</remarks>
            [JsonPropertyName("saturation")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Saturation { get; set; }

            /// <summary>
            /// Color temperature, Kelvins.
            /// </summary>
            /// <remarks>
            /// Range 2500..6500. Set to 0 to apply specified hue and saturation.
            /// </remarks>
            [JsonPropertyName("color_temp")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? ColorTemperature { get; set; }
        }

        public class Result
        {

        }

        internal static TapoRequest<Params> CreateRequest(Params parameters) =>
            Utils.CreateTapoRequest("set_device_info", parameters);

        internal static TapoRequest<ParamsBulb> CreateRequest(ParamsBulb parameters) =>
            Utils.CreateTapoRequest("set_device_info", parameters);
    }
}
