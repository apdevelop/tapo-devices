using System;

namespace TapoDevices
{
    public class TapoDeviceFactory
    {
        private readonly string username;

        private readonly string password;

        public TapoDeviceFactory(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public TapoBulb CreateBulb(string ipAddress) => 
            new TapoBulb(ipAddress, this.username, this.password);

        public TapoBulb CreateBulb(string ipAddress, TimeSpan defaultRequestTimeout) =>
            new TapoBulb(ipAddress, this.username, this.password, defaultRequestTimeout);

        public TapoPlug CreatePlug(string ipAddress) =>
            new TapoPlug(ipAddress, this.username, this.password);

        public TapoPlug CreatePlug(string ipAddress, TimeSpan defaultRequestTimeout) =>
            new TapoPlug(ipAddress, this.username, this.password, defaultRequestTimeout);
    }
}
