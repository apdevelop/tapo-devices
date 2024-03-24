using System;
using System.Text.Json.Serialization;

namespace TapoDevices
{
    public class GetEnergyUsage
    {
        public class Params
        {

        }

        public class Result
        {

        }

        public class ResultPlug : Result
        {
            /// <summary>
            /// Current day runtime, in minutes.
            /// </summary>
            [JsonPropertyName("today_runtime")]
            public int TodayRuntimeMinutes { get; set; }

            /// <summary>
            /// Current day runtime.
            /// </summary>
            public TimeSpan TodayRuntime => TimeSpan.FromMinutes(this.TodayRuntimeMinutes);

            /// <summary>
            /// Current month runtime, in minutes.
            /// </summary>
            [JsonPropertyName("month_runtime")]
            public int MonthRuntimeMinutes { get; set; }

            /// <summary>
            /// Current month runtime.
            /// </summary>
            public TimeSpan MonthRuntime => TimeSpan.FromMinutes(this.MonthRuntimeMinutes);

            /// <summary>
            /// Current day energy, Wh.
            /// </summary>
            [JsonPropertyName("today_energy")]
            public int TodayEnergy { get; set; }

            /// <summary>
            /// Current day energy, kWh.
            /// </summary>
            public double TodayEnergykWh => this.TodayEnergy / 1000.0;

            /// <summary>
            /// Current month energy, Wh.
            /// </summary>
            [JsonPropertyName("month_energy")]
            public int MonthEnergy { get; set; }

            /// <summary>
            /// Current month energy, kWh.
            /// </summary>
            public double MonthEnergykWh => this.MonthEnergy / 1000.0;

            /// <summary>
            /// Current power, mW.
            /// </summary>
            [JsonPropertyName("current_power")]
            public int CurrentPowerMilliwatts { get; set; }

            /// <summary>
            /// Current power, W.
            /// </summary>
            public double CurrentPowerWatts => this.CurrentPowerMilliwatts / 1000.0;
        }

        internal static TapoRequest<Params> CreateRequest() =>
            Utils.CreateTapoRequest<Params>("get_energy_usage", null);
    }
}
