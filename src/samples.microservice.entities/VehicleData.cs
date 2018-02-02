using Newtonsoft.Json;
using samples.microservice.core;

namespace samples.microservice.entities
{
    public class VehicleData : Entity
    {
        [JsonProperty(PropertyName = "vin")]
        public string Vin { get; set; }

        [JsonProperty(PropertyName = "esn")]
        public string Esn { get; set; }

        [JsonProperty(PropertyName = "fuel")]
        public int FuelGauge { get; set; }

        [JsonProperty(PropertyName = "pressure")]
        public int PressureGauge { get; set; }

        [JsonProperty(PropertyName = "indicatorlight")]
        public bool IndicatorLights { get; set; }

        [JsonProperty(PropertyName = "headlight")]
        public bool HeadLights { get; set; }
    }
}