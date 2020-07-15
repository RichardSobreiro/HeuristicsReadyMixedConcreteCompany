using Entity;
using System;
using System.Collections.Generic;

namespace Repository.Interfaces
{
    public interface IDeliveryOrderRepository
    {
        List<Delivery> GetDeliveriesOrdersWithDeliveryOrderTrips(int instanceNumber, DateTime begin, DateTime end);
    }
}
