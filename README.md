# Tapo Devices

The C# .NET5 library for basic controlling of some types of **TP-Link Tapo** devices 
in local network without using TP-Link Cloud features. Device must be registered and has known IP address.

This project is mostly experimental and based on undocumented API features.


### Usage

```c#
// Tapo account credentials (email and password)
var factory = new TapoDevices.TapoDeviceFactory("user@domain.com", "password");

// 1. Control the Smart Socket with specified IP address
var plug = factory.CreatePlug("192.168.1.71");
await plug.ConnectAsync();

// Get device information
var info = await plug.GetInfoAsync();
var energyUsage = await plug.GetEnergyUsageAsync();

// Turn device on and then off
await plug.TurnOnAsync();
await plug.TurnOffAsync();

// 2. Control the Light Bulb
var bulb = factory.CreateBulb("192.168.1.72");
await bulb.ConnectAsync();
await bulb.SetColorTemperatureAsync(2700);
await bulb.SetBrightnessAsync(50);
await bulb.SetParametersAsync(
    new TapoDevices.SetDeviceInfo.ParamsBulb
    {
        Brightness = 5,
        Hue = 120,
        Saturation = 100,
        ColorTemperature = 0,
    });
```

### Supported and tested devices

| Device     |   Description                                                                             |   FW Version                     | 
|:----------:|:------------------------------------------------------------------------------------------|:--------------------------------:|
| **P110**  |[Mini Smart Wi-Fi Socket, Energy Monitoring](https://www.tp-link.com/en/home-networking/smart-plug/tapo-p110/) | 1.3.0 Build 230905 Rel.152200  |
| **P300**  |[Smart Wi-Fi Power Strip](https://www.tp-link.com/en/home-networking/smart-plug/tapo-p300/) | 1.0.15 Build 231130 Rel.122554  |
| **L510**  |[Smart Wi-Fi Light Bulb, Dimmable](https://www.tp-link.com/en/home-networking/smart-bulb/tapo-l510e/)    | 1.1.0 Build 230721 Rel.224802 |
| **L530**  |[Smart Wi-Fi Light Bulb, Multicolor](https://www.tp-link.com/en/home-networking/smart-bulb/tapo-l530e/)  | 1.1.0 Build 230721 Rel.224802 |
| **L535B**  |[Smart Wi-Fi Light Bulb, Multicolor](https://www.tp-link.com/en/home-networking/smart-bulb/tapo-l535b/)  | 1.1.5 Build 240328 Rel.194528 |

### Features
* Asynchronous only set of methods for devices control
* Generic types for requests/responses
* NET 5.0 assembly without additional dependencies

### TODOs
* Implement as .NET8 / .NET Standard 2.0 library
* Support for more of device features
* Support for more device types
* API documentation
* Nuget package

### References
* [TapoConnect](https://github.com/cwakefie27/TapoConnect)
* [TP-Link Tapo devices support for MajorDoMo](https://github.com/sergejey/majordomo-tapo/tree/main)
* [Kostiantyn's Blog: Connecting to Tapo lamps from F#](https://sharovarskyi.com/blog/posts/fsharp-tapo-lamps/)
* [KHome.TapoLights](https://github.com/kostya9/KHome.TapoLights)
* [PyP100 Python library](https://github.com/fishbigger/TapoP100)
* [TapoSharp is a .NET implementation of the Tapo Smart Plug API](https://github.com/Cirx08/TapoSharp)
* [TapoPlugAPI](https://pypi.org/project/tapo-plug/)
