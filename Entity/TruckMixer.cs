using System;
using System.Collections.Generic;

namespace Entity
{
    public class TruckMixer
    {
        public int TruckMixerId { get; set; }
        public int InitialLoadingPlaceId { get; set; }
        public float Capacity { get; set; }
        public DateTime EndLastService { get; set; }
        public List<Route> Routes { get; set; }
    }
}
