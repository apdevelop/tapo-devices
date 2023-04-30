using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TapoDevices
{
    class SetDeviceInfo
    {
        public class Params
        {
            [JsonPropertyName("device_on")]
            public bool DeviceOn { get; set; } // TODO: more nullable fields
        }

        public class Result
        {

        }

        internal static TapoRequest<IDictionary<string, object>> CreateRequest(IDictionary<string, object> parameters) => // TODO: ? typed params to set several values at once
            Utils.CreateTapoRequest("set_device_info", parameters);

        internal static TapoRequest<Params> CreateRequest(Params parameters) =>
            Utils.CreateTapoRequest("set_device_info", parameters);
    }
}
