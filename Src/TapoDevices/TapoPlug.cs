using System;
using System.Threading.Tasks;

namespace TapoDevices
{
    /// <summary>
    /// Represents connection to Tapo Smart Wi-Fi Socket P100/P110/P300 device.
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
            TimeSpan defaultTimeout) : base(ipAddress, username, password, defaultTimeout)
        {

        }

        public async Task<GetDeviceUsage.ResultPlug> GetDeviceUsageAsync()
        {
            var request = GetDeviceUsage.CreateRequest();
            return await PostSecuredAsync<TapoRequest<GetDeviceUsage.Params>, GetDeviceUsage.ResultPlug>(request);
        }

        public async Task<GetEnergyUsage.ResultPlug> GetEnergyUsageAsync()
        {
            var request = GetEnergyUsage.CreateRequest();
            return await PostSecuredAsync<TapoRequest<GetEnergyUsage.Params>, GetEnergyUsage.ResultPlug>(request);
        }

        public async Task<GetLedInfo.ResultPlug> GetLedInfoAsync()
        {
            var request = GetLedInfo.CreateRequest();
            return await PostSecuredAsync<TapoRequest<GetLedInfo.Params>, GetLedInfo.ResultPlug>(request);
        }
    }
}
