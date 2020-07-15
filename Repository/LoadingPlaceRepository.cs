using Dapper;
using Entity;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class LoadingPlaceRepository : ILoadingPlaceRepository
    {
        public List<LoadingPlace> GetLoadingPlaces(int instanceNumber, DateTime begin, DateTime end)
        {
            StringBuilder query = new StringBuilder("");
            query.Append("SELECT ");
            query.Append("lc.\"LocationId\" AS \"LoadingPlaceId\", ");
            query.Append("lc.\"InstanceNumber\", ");
            query.Append("lc.\"Latitude\", ");
            query.Append("lc.\"Longitude\", ");
            query.Append("lc.\"ReferenceNumber\", ");
            query.Append("v.\"VehicleId\", ");
            query.Append("lc.\"LocationId\" AS \"InitialLoadingPlaceId\", ");
            query.Append("v.\"MaintenanceCostPerKm\", ");
            query.Append("v.\"Volume\", ");
            query.Append("v.\"FuelConsumptionPerKm\" ");
            query.Append("FROM public.\"Location\" AS lc ");
            query.Append("INNER JOIN public.\"Vehicle\" AS v ON v.\"LocationId\" = lc.\"LocationId\" ");
            query.Append("WHERE lc.\"InstanceNumber\" = @InstanceNumber AND lc.\"Kind\" = 1 ");

            List<LoadingPlace> loadingPlaces = new List<LoadingPlace>();
            using (NpgsqlConnection connection = new NpgsqlConnection(
                _configuration.GetValue<string>("ConnectionStrings:READYMIXEDCONCRETEDELIVERYPROBLEMDB")))
            {
                connection.Query<LoadingPlace, TruckMixer, LoadingPlace>(
                    query.ToString(),
                    (lc, v) =>
                    {
                        LoadingPlace newLoadingPlace;
                        if (!loadingPlaces.Any(lp => lp.LoadingPlaceId == lc.LoadingPlaceId))
                        {
                            newLoadingPlace = lc;
                            newLoadingPlace.TruckMixers = new List<TruckMixer>();
                            loadingPlaces.Add(newLoadingPlace);
                        }

                        lc.TruckMixers.Add(v);

                        return lc;
                    },
                    param: new { InstanceNumber = instanceNumber },
                    splitOn: "VehicleId");
                return loadingPlaces;
            }
        }

        public LoadingPlaceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IConfiguration _configuration;
    }
}
