using System.Collections.Generic;
using samples.microservice.core;

namespace samples.microservice.entities
{
    public class MyDocument: Entity
    {
        public List<string> Items { get; set; }
    }
}