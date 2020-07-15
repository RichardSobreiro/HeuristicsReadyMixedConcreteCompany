using Entity;
using System.Collections.Generic;

namespace Business.Extensions
{
    public static class DeliveryOrderExtension
    {
        public static List<Delivery> GetDeliveries(this DeliveryOrder deliveryOrderDto)
        {
            return new List<Delivery>();
        }
    }
}
