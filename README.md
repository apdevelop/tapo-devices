# Tapo Devices

The C# .NET5 library for basic controlling of some types of **TP-Link Tapo** devices 
in local network (not using TP-Link Cloud features). 
Device must be registered and has known IP address.

### Usage

```c#
// Connect to device with specified IP address, using Tapo account credentials (email and password)
var device = new TapoDevices.TapoDevice("192.168.1.72");
await device.ConnectAsync("user@domain.com", "password");

// Get device information
var info = await device.GetInfoAsync();

// Turn device on and then off
await device.TurnOnAsync();
await device.TurnOffAsync();
```


### Supported and tested devices
* **P110** [Mini Smart Wi-Fi Socket, Energy Monitoring](https://www.tp-link.com/en/home-networking/smart-plug/tapo-p110/)
* **L530** [Smart Wi-Fi Light Bulb, Multicolor](https://www.tp-link.com/en/home-networking/smart-bulb/tapo-l530e/)


### Features
* Asynchronous only set of methods for devices control
* Generic types for requests/responses
* NET 5.0 assembly without additional depandencies


### TODOs
* Implement as .NET Standard 2.0 library
* Support for more of device features
* Support for more device types


### References
* [Kostiantyn's Blog: Connecting to Tapo lamps from F#](https://sharovarskyi.com/blog/posts/fsharp-tapo-lamps/)
* [KHome.TapoLights](https://github.com/kostya9/KHome.TapoLights)
* [PyP100 Python library](https://github.com/fishbigger/TapoP100)
