using System;
using System.Collections.Generic;

namespace Entity
{
    public class TruckMixer
    {
        public int TruckMixerId { get; set; }
        public int InitialLoadingPlaceId { get; set; }
        public double MaintenanceCostPerKm { get; set; } = 0.1d;
        public double FuelConsumptionKmPerLiter { get; set; } = 4d;
        public float Capacity { get; set; } = 8;
        public bool Available { get; set; }

        public DateTime EndLastService { get; set; }
        public List<Route> Routes { get; set; }
    }
}
