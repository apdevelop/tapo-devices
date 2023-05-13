using System;
using System.Threading.Tasks;

namespace TapoDevices
{
    /// <summary>
    /// Represents connection to Tapo Smart Wi-Fi Socket P100/P110 device.
    /// </summary>
    /// <remarks>SMART.TAPOPLUG</remarks>
    public class TapoPlug : TapoDevice
    {
        public TapoPlug(
            string ipAddress,
            string username,
            string password) : base(ipAddress, username, password)
        {

        }

        public TapoPlug(
            string ipAddress,
            string username,
            string password,
            TimeSpan defaultRequestTimeout) : base(ipAddress, username, password, defaultRequestTimeout)
        {

        }

        public async Task<GetEnergyUsage.ResultPlug> GetEnergyUsageAsync()
        {
            var request = GetEnergyUsage.CreateRequest();
            return await PostSecuredAsync<TapoRequest<GetEnergyUsage.Params>, GetEnergyUsage.ResultPlug>(request);
        }
    }
}
