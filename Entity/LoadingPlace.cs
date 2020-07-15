using System.Collections.Generic;

namespace Entity
{
    public class LoadingPlace
    {
        public int LoadingPlaceId { get; set; }
        public int InstanceNumber { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public List<TruckMixer> TruckMixers { get; set; }
    }
}
