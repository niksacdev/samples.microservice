using System.Collections.Generic;
using Newtonsoft.Json;

namespace samples.microservice.core
{
    public abstract class Entity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "custom")]
        public Dictionary<string, string> CustomProperties { get; set; }

        /// <summary>
        /// serialize JSON into string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}