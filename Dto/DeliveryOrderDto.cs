using System;

namespace Dto
{
    public class DeliveryOrderDto
    {
        public int? DeliveryOrderId { get; set; }
        public float TotalVolume { get; set; }
        public int RMCType { get; set; }
        public DateTime BeginTime { get; set; }
        public int Interval { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int RegionId { get; set; }

        public class DeliveryDto
        {
            public int TripDtoId { get; set; }
            public DateTime ServiceTime { get; set; }
            public float Volume { get; set; }
        }

    }
}
