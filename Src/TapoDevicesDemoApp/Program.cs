using System;
using System.Threading.Tasks;

namespace TapoDevicesDemoApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Tapo account credentials (email and password) from command line
            var factory = new TapoDevices.TapoDeviceFactory(args[0], args[1]);

            // Connect to device with specified IP address
            var plug = factory.CreatePlug("192.168.1.69", TimeSpan.FromSeconds(1));
            await plug.ConnectAsync();
            var plugInfo = await plug.GetInfoAsync();
            Console.WriteLine($"{plugInfo.Type}  {plugInfo.Model}  '{plugInfo.Nickname}'");
            Console.WriteLine($"HW={plugInfo.HardwareVersion}  FW={plugInfo.FirmwareVersion}");
            Console.WriteLine($"IP={plugInfo.IPAddress}  MAC={plugInfo.MacAddress}  SSID ='{plugInfo.SSID}'  RSSI={plugInfo.Rssi}");

            await plug.TurnOnAsync();

            var plugEnergy = await plug.GetEnergyUsageAsync();
            Console.WriteLine($"Power={plugEnergy.CurrentPowerWatts:0.0} W");
            Console.WriteLine($" Today: {plugEnergy.TodayEnergykWh:0.000} kWh  {plugEnergy.TodayRuntime.TotalHours:0.0} h");
            Console.WriteLine($" Month: {plugEnergy.MonthEnergykWh:0.000} kWh  {plugEnergy.MonthRuntime.TotalHours:0.0} h");

            await plug.TurnOffAsync();
            await plug.TurnOnWithDelayAsync(TimeSpan.FromSeconds(5));
            await Task.Delay(TimeSpan.FromSeconds(2));
            var rules = await plug.GetCountdownRulesAsync();
            Console.WriteLine($"Rule: '{rules.Rules[0].Id}' remain: {rules.Rules[0].Remain} s");

            await plug.TurnOffWithDelayAsync(TimeSpan.FromSeconds(5));

            Console.ReadLine();

            // Connect to device with specified IP address
            var bulb = factory.CreateBulb("192.168.1.79");
            await bulb.ConnectAsync();

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
