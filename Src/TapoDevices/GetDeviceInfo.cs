﻿using System;
using System.Text;
using System.Text.Json.Serialization;

namespace TapoDevices
{
    public class GetDeviceInfo
    {
        public class Result
        {
            [JsonPropertyName("device_id")]
            public string DeviceId { get; set; }

            [JsonPropertyName("fw_ver")]
            public string FirmwareVersion { get; set; }

            [JsonPropertyName("hw_ver")]
            public string HardwareVersion { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("model")]
            public string Model { get; set; }

            [JsonPropertyName("mac")]
            public string MacAddress { get; set; }

            [JsonPropertyName("hw_id")]
            public string HardwareId { get; set; }

            [JsonPropertyName("fw_id")]
            public string FirmwareId { get; set; }

            [JsonPropertyName("oem_id")]
            public string OemId { get; set; }

            [JsonPropertyName("ip")]
            public string IPAddress { get; set; }

            [JsonPropertyName("time_diff")]
            public int TimeDifference { get; set; }

            [JsonPropertyName("ssid")]
            public string SSIDEncoded { get; set; }

            [JsonIgnore]
            public string SSID => Encoding.UTF8.GetString(Convert.FromBase64String(this.SSIDEncoded));

            [JsonPropertyName("rssi")]
            public int Rssi { get; set; }

            [JsonPropertyName("signal_level")]
            public int SignalLevel { get; set; }

            [JsonPropertyName("latitude")]
            public double Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double Longitude { get; set; }

            [JsonPropertyName("lang")]
            public string Language { get; set; }

            [JsonPropertyName("avatar")]
            public string Avatar { get; set; }

            [JsonPropertyName("region")]
            public string Region { get; set; }

            [JsonPropertyName("specs")]
            public string Specs { get; set; }

            [JsonPropertyName("nickname")]
            public string NicknameEncoded { get; set; }

            [JsonIgnore]
            public string Nickname => Encoding.UTF8.GetString(Convert.FromBase64String(this.NicknameEncoded));

            [JsonPropertyName("has_set_location_info")]
            public bool HasSetLocationInfo { get; set; }

            [JsonPropertyName("device_on")]
            public bool DeviceIsOn { get; set; }
        }

        // TODO: specific devices results (light bulb, socket)

        internal static TapoRequest<object> CreateRequest() =>
            Utils.CreateTapoRequest<object>("get_device_info", null);
    }
}
