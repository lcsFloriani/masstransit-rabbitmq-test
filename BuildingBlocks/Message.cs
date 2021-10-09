using System;

namespace BuildingBlocks
{
    public class Message
    {
        public Guid RoutingKey { get; set; }
        public string Value { get; set; }
    }
}
