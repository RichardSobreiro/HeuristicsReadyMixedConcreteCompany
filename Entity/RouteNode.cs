using GeoCoordinatePortable;
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
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public GeoCoordinate GeoCordinates { get { return new GeoCoordinate(Latitude, Longitude); } }
    }
}
