using System;

namespace Entity
{
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public int DeliveryOrderId { get; set; }
        public int InstanceNumber { get; set; }
        public DateTime ServiceTimeBegin { get; set; }
        public float Volume { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float CustomerRmcDischargeRate { get; set; } = 2;
        public int RmcType { get; set; }


        public int LoadingPlaceId { get; set; }
        public int? RouteId { get; set; }

        public int ClientId { get; set; }
        public int Interval { get; set; }
        public int DischargeDuration { get; set; }
        public decimal Income { get; set; }
        public decimal RMCCost { get; set; }
    }
}
