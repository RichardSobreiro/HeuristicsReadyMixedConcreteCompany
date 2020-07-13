using System;
using System.Collections.Generic;

namespace Entity
{
    public class DeliveryOrder
    {
        public int OrderId { get; set; }
        public float TotalRevenue { get; set; }
        public float TotalVolume { get; set; }
        public int RmcType { get; set; }
        public DateTime BeginTime { get; set; }
        public int Interval { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        

        public List<Delivery> Deliveries { get; set; }
    }
}
