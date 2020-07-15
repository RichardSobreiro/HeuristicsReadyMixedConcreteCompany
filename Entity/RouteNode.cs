using System;

namespace Entity
{
    public class RouteNode
    {
        public int RouteNodeId { get; set; }
        public int RouteId { get; set; }
        public int DeliveryOrderId { get; set; }
        public int DeliveryId { get; set; }
        public DateTime BeginService { get; set; }
        public DateTime EndService { get; set; }
    }
}
