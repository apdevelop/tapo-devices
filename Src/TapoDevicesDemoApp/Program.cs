using System;
using System.Threading.Tasks;

namespace TapoDevicesDemoApp
{
    class Program
    {
        private static TapoDevices.TapoDeviceFactory factory;

        static async Task Main(string[] args)
        {
            // Tapo account credentials (email and password) from command line
            factory = new TapoDevices.TapoDeviceFactory(args[0], args[1]);

            await ControlBulb("192.168.1.90");
            await ControlPlug("192.168.1.91");
        }

        private static async Task ControlPlug(string ipAddress)
        {
            // Connect to device with specified IP address
            var plug = factory.CreatePlug(ipAddress, TimeSpan.FromSeconds(3));
            await plug.ConnectAsync();

            var plugInfo = await plug.GetInfoAsync();
            Console.WriteLine($"{plugInfo.Type}  {plugInfo.Model}  '{plugInfo.Nickname}'");
            Console.WriteLine($"HW={plugInfo.HardwareVersion}  FW={plugInfo.FirmwareVersion}");
            Console.WriteLine($"IP={plugInfo.IPAddress}  MAC={plugInfo.MacAddress}  SSID ='{plugInfo.SSID}'  RSSI={plugInfo.Rssi}");

            var deviceTime = await plug.GetDeviceTimeAsync();
            Console.WriteLine($"Time={DateTimeOffset.FromUnixTimeSeconds(deviceTime.Timestamp).LocalDateTime} UTC{(deviceTime.TimeDiff >= 0 ? "+" : "")}{deviceTime.TimeDiff / 60} '{deviceTime.Region}'");

            var ledInfo = await plug.GetLedInfoAsync();
            Console.WriteLine($"LED={ledInfo.LedStatus} RULE='{ledInfo.LedRule}'");

            var usage = await plug.GetDeviceUsageAsync();
            Console.WriteLine($"Usage={usage.TimeUsage.Today}/{usage.TimeUsage.Past7}/{usage.TimeUsage.Past30}");

            await plug.TurnOnAsync(); // TODO: P300 support with child devices
                                  
            if (plugInfo.Model == "P110") // P100 and P300 do not support Energy Usage
            {
                var plugEnergy = await plug.GetEnergyUsageAsync(); // TODO: feature support checking inside TapoPlug class
                Console.WriteLine($"Power={plugEnergy.CurrentPowerWatts:0.0} W");
                Console.WriteLine($" Today: {plugEnergy.TodayEnergykWh:0.000} kWh  {plugEnergy.TodayRuntime.TotalHours:0.0} h");
                Console.WriteLine($" Month: {plugEnergy.MonthEnergykWh:0.000} kWh  {plugEnergy.MonthRuntime.TotalHours:0.0} h");
            }

            await plug.TurnOffAsync();
            await plug.TurnOnWithDelayAsync(TimeSpan.FromSeconds(5)); // TODO: ! P300 needs child control
            await Task.Delay(TimeSpan.FromSeconds(2));
            var rules = await plug.GetCountdownRulesAsync();
            if (rules.Rules.Length > 0)
            {
                Console.WriteLine($"Rule: '{rules.Rules[0].Id}' remain: {rules.Rules[0].Remain} s");
            }

            await plug.TurnOffWithDelayAsync(TimeSpan.FromSeconds(5));
        }

        private static async Task ControlBulb(string ipAddress)
        {
            // Connect to device with specified IP address
            var bulb = factory.CreateBulb(ipAddress);
            await bulb.ConnectAsync();

            // Read and display device information
            var info = await bulb.GetInfoAsync();
            Console.WriteLine($"{info.Type}  {info.Model}  '{info.Nickname}'");
            Console.WriteLine($"HW={info.HardwareVersion}  FW={info.FirmwareVersion}");
            Console.WriteLine($"IP={info.IPAddress}  MAC={info.MacAddress}  SSID ='{info.SSID}'  RSSI={info.Rssi}");
            Console.WriteLine($"Brightness={info.Brightness}  Color={info.ColorTemperature}  Color range={info.ColorTemperatureRange[0]}..{info.ColorTemperatureRange[1]}");

            var deviceTime = await bulb.GetDeviceTimeAsync();
            Console.WriteLine($"Time={DateTimeOffset.FromUnixTimeSeconds(deviceTime.Timestamp).LocalDateTime} UTC{(deviceTime.TimeDiff >= 0 ? "+" : "")}{deviceTime.TimeDiff / 60} '{deviceTime.Region}'");

            var usage = await bulb.GetDeviceUsageAsync();
            Console.WriteLine($"Usage={usage.TimeUsage.Today}/{usage.TimeUsage.Past7}/{usage.TimeUsage.Past30}");

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
        }
    }
}
