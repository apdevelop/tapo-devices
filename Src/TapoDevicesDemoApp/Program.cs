using System;
using System.Threading.Tasks;

namespace TapoDevicesDemoApp
{
    class Program
    {
        static async Task Main()
        {
            // Connect to device with specified IP address, using Tapo account credentials (email and password)
            var device = new TapoDevices.TapoDevice("192.168.1.72");
            await device.ConnectAsync("user@domain.com", "password");

            // Read and display device information
            var info = await device.GetInfoAsync();
            Console.WriteLine($"{info.Model} {info.Type} {info.HardwareVersion} '{info.Nickname}'");
            Console.WriteLine($"{info.IPAddress} {info.MacAddress}");
            Console.WriteLine($"SSID ='{info.SSID}' RSSI={info.Rssi}");

            // Controling the device - turn it on and then off
            await device.TurnOnAsync();
            await Task.Delay(TimeSpan.FromSeconds(3));
            await device.TurnOffAsync();

            Console.ReadLine();
        }
    }
}
