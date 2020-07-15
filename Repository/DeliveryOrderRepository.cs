using Dapper;
using Entity;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class DeliveryOrderRepository : IDeliveryOrderRepository
    {
        public List<Delivery> GetDeliveriesOrdersWithDeliveryOrderTrips(int instanceNumber, DateTime begin, DateTime end)
        {
            StringBuilder query = new StringBuilder("");
            query.Append("SELECT ");
            query.Append("dlot.\"DeliveryOrderId\", ");
            query.Append("dlot.\"DeliveryOrderTripId\" AS \"DeliveryId\", ");
            query.Append("dlot.\"ReadyMixedConcreteId\" AS \"RmcType\", ");
            query.Append("dlot.\"RequestedTime\" AS \"ServiceTimeBegin\", ");
            query.Append("dlot.\"Interval\", ");
            query.Append("dlot.\"DischargeDuration\", ");
            query.Append("dlot.\"Volume\", ");
            query.Append("dlot.\"ClientId\", ");
            query.Append("dlot.\"Income\", ");
            query.Append("dlot.\"RMCCost\", ");
            query.Append("const.\"LocationId\", ");
            query.Append("const.\"InstanceNumber\", ");
            query.Append("const.\"Latitude\", ");
            query.Append("const.\"Longitude\" ");
            query.Append("FROM ");
            query.Append("public.\"DeliveryOrderTrip\" AS dlot ");
            query.Append("INNER JOIN public.\"Location\" AS const ON const.\"LocationId\" = dlot.\"LocationId\" AND const.\"Kind\" = 2 ");
            query.Append("WHERE dlot.\"InstanceNumber\" = @InstanceNumber ");
            query.Append("AND dlot.\"RequestedTime\" >= @Begin ");
            query.Append("AND dlot.\"RequestedTime\" <= @End ");

            using (NpgsqlConnection connection = new NpgsqlConnection(
                _configuration.GetValue<string>("ConnectionStrings:READYMIXEDCONCRETEDELIVERYPROBLEMDB")))
            {
                return connection.Query<Delivery>(
                    query.ToString(),
                    param: new { InstanceNumber = instanceNumber, Begin = begin, End = end }
                ).AsList();
            }
        }

        public DeliveryOrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IConfiguration _configuration;
    }
}
