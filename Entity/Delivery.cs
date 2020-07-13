using System;

namespace Entity
{
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public int OrderId { get; set; }
        public DateTime ServiceTimeBegin { get; set; }
        public float Volume { get; set; }
        public int LoadingPlaceId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float CustomerRmcDischargeRate { get; set; }
        public int RmcType { get; set; }

        public int? RouteId { get; set; }
    }
}
