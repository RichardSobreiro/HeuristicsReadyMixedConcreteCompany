using Entity;
using System;
using System.Collections.Generic;

namespace Repository.Interfaces
{
    public interface ILoadingPlaceRepository
    {
        List<LoadingPlace> GetLoadingPlaces(int instanceNumber, DateTime begin, DateTime end);
    }
}
