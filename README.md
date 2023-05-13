# Tapo Devices

The C# .NET5 library for basic controlling of some types of **TP-Link Tapo** devices 
in local network (not using TP-Link Cloud features). 
Device must be registered and has known IP address.

### Usage

```c#
// Connect to device with specified IP address, using Tapo account credentials (email and password)
var device = new TapoDevices.TapoDevice("192.168.1.71");
await device.ConnectAsync("user@domain.com", "password");

// Get device information
var info = await device.GetInfoAsync();

// Turn device on and then off
await device.TurnOnAsync();
await device.TurnOffAsync();


var bulb = new TapoDevices.TapoBulb("192.168.1.72");
await bulb.ConnectAsync("user@domain.com", "password");
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
* **P110** [Mini Smart Wi-Fi Socket, Energy Monitoring](https://www.tp-link.com/en/home-networking/smart-plug/tapo-p110/)
* **L510** [Smart Wi-Fi Light Bulb, Dimmable](https://www.tp-link.com/en/home-networking/smart-bulb/tapo-l510e/)
* **L530** [Smart Wi-Fi Light Bulb, Multicolor](https://www.tp-link.com/en/home-networking/smart-bulb/tapo-l530e/)


### Features
* Asynchronous only set of methods for devices control
* Generic types for requests/responses
* NET 5.0 assembly without additional dependencies


### TODOs
* Implement as .NET Standard 2.0 library
* Support for more of device features
* Support for more device types


### References
* [Kostiantyn's Blog: Connecting to Tapo lamps from F#](https://sharovarskyi.com/blog/posts/fsharp-tapo-lamps/)
* [KHome.TapoLights](https://github.com/kostya9/KHome.TapoLights)
* [PyP100 Python library](https://github.com/fishbigger/TapoP100)
* [TapoSharp is a .NET implementation of the Tapo Smart Plug API](https://github.com/Cirx08/TapoSharp)
* 