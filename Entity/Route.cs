using System;
using System.Collections.Generic;

namespace Entity
{
    public class Route
    {
        public int RouteId { get; set; }
        public int TruckMixerId { get; set; }
        public int RmcType { get; set; }
        public float TotalVolume { get; set; }

        public int InitialLoadingPlaceId { get; set; }
        public int FinalLoadingPlaceId { get; set; }

        public DateTime LoadingBegin { get; set; }
        public DateTime LoadingEnd { get; set; }
        public List<RouteNode> RouteNodes { get; set; }
        public DateTime ReturnTime { get; set; }

        public int NextRouteNodeId { get; set; } = 1;
    }
}
