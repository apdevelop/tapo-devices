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

            public TimeSpan TodayRuntime => TimeSpan.FromMinutes(this.TodayRuntimeMinutes);

            /// <summary>
            /// Current month runtime, in minutes.
            /// </summary>
            [JsonPropertyName("month_runtime")]
            public int MonthRuntimeMinutes { get; set; }

            public TimeSpan MonthRuntime => TimeSpan.FromMinutes(this.MonthRuntimeMinutes);

            /// <summary>
            /// Current day energy, Wh.
            /// </summary>
            [JsonPropertyName("today_energy")]
            public int TodayEnergy { get; set; }

            public double TodayEnergykWh => this.TodayEnergy / 1000.0;

            /// <summary>
            /// Current month energy, Wh.
            /// </summary>
            [JsonPropertyName("month_energy")]
            public int MonthEnergy { get; set; }

            public double MonthEnergykWh => this.MonthEnergy / 1000.0;

            [JsonPropertyName("current_power")]
            public int CurrentPowerMilliwatts { get; set; }

            public double CurrentPowerWatts => this.CurrentPowerMilliwatts / 1000.0;
        }

        internal static TapoRequest<Params> CreateRequest() =>
            Utils.CreateTapoRequest<Params>("get_energy_usage", null);
    }
}
