using System;
using System.Threading.Tasks;

namespace TapoDevicesDemoApp
{
    class Program
    {
        static async Task Main()
        {
            // Connect to device with specified IP address, using Tapo account credentials (email and password)
            var bulb = new TapoDevices.TapoBulb("192.168.1.79");
            await bulb.ConnectAsync("user@domain.com", "password");

            // Read and display device information
            var info = await bulb.GetInfoAsync();
            Console.WriteLine($"{info.Type}  {info.Model}  '{info.Nickname}'");
            Console.WriteLine($"HW={info.HardwareVersion}  FW={info.FirmwareVersion}");
            Console.WriteLine($"IP={info.IPAddress}  MAC={info.MacAddress}  SSID ='{info.SSID}'  RSSI={info.Rssi}");
            Console.WriteLine($"Brightness={info.Brightness}  Color={info.ColorTemperature}  Color range={info.ColorTemperatureRange[0]}..{info.ColorTemperatureRange[1]}");

            // Controling the device - turn it on, set parameters and then turn off
            await bulb.TurnOnAsync();
            await bulb.SetColorTemperatureAsync(2700);
            await Task.Delay(TimeSpan.FromSeconds(2));
            await bulb.SetBrightnessAsync(50);
            await Task.Delay(TimeSpan.FromSeconds(2));
            await bulb.SetColorAsync(270, 50);
            await Task.Delay(TimeSpan.FromSeconds(2));

            await bulb.SetParametersAsync(
                new TapoDevices.SetDeviceInfo.ParamsBulb
                {
                    Brightness = 5,
                    Hue = 120,
                    Saturation = 100,
                    ColorTemperature = 0, // To apply hue and saturation
                });

            await bulb.TurnOffAsync();

            Console.ReadLine();
        }
    }
}
