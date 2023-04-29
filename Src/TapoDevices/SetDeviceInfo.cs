using System.Collections.Generic;

namespace TapoDevices
{
    class SetDeviceInfo
    {
        public class Result
        {

        }

        internal static TapoRequest<IDictionary<string, object>> CreateRequest(IDictionary<string, object> parameters) =>
            Utils.CreateTapoRequest("set_device_info", parameters);
    }
}
