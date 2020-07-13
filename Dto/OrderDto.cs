using System;

namespace Dto
{
    public class OrderDto
    {
        public int? OrderDtoId { get; set; }
        public float TotalVolume { get; set; }
        public int RMCType { get; set; }
        public DateTime BeginTime { get; set; }
        public int Interval { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int RegionId { get; set; }

        public class TripDto
        {
            public int TripDtoId { get; set; }
            public DateTime ArrivalTime { get; set; }
            public float Volume { get; set; }
        }

        public class CustomerDto
        {
            public int CustomerDtoId { get; set; }

        }
    }
}
