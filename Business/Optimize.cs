using Business.Extensions;
using Dto;
using Entity;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    public class Optimize
    {
        public void Execute(OrderDto orderDto)
        {
            List<LoadingPlace> loadingPlaces = GetLoadingPlaces();
            List<DeliveryOrder> delieveryOrders = GetOrders();

            List<Delivery> deliveries = new List<Delivery>();
            foreach(var deliveryOrderLoadingPlace in delieveryOrders)
            {
                deliveries.AddRange(deliveryOrderLoadingPlace.Deliveries);
            }
            deliveries = deliveries.OrderBy(t => t.ServiceTimeBegin).ToList();

            int noScheduled = deliveries.Count;
            while(noScheduled > 0)
            {
                Delivery delivery = deliveries.FirstOrDefault();

                Route bestRouteToAttend = FindBestRouteToAttendDelivery(loadingPlaces);

                if(bestRouteToAttend == null)
                {
                    TruckMixer truckMixer = FindBestTruckMixerToAttendDelivery(loadingPlaces);
                    int elp
                    Route newRoute = new Route() 
                    { 
                        TruckMixerId = truckMixer.TruckMixerId,
                        RmcType = delivery.RmcType,
                        TotalVolume = delivery.Volume,
                        InitialLoadingPlaceId = truckMixer.InitialLoadingPlaceId,
                        FinalLoadingPlaceId = truckMixer.InitialLoadingPlaceId,
                        LoadingEnd = delivery.ServiceTimeBegin.AddMinutes(-1 * delivery.GetTravelTime(
                            loadingPlaces.FirstOrDefault(lp => lp.LoadingPlaceId == bestRouteToAttend.InitialLoadingPlaceId)))
                    };
                }
                else
                {
                    bestRouteToAttend.TotalVolume += delivery.Volume;
                    bestRouteToAttend.LoadingEnd.AddMinutes(delivery.Volume);
                    bestRouteToAttend.RouteNodes.Add(new RouteNodes()
                    {
                        RouteId = bestRouteToAttend.RouteId,
                        BeginService = delivery.ServiceTimeBegin,
                        EndService = delivery.ServiceTimeBegin.AddMinutes(delivery.Volume * delivery.CustomerRmcDischargeRate)
                    });
                    int travelReturnTime = delivery.GetTravelTime(
                        loadingPlaces.FirstOrDefault(lp => lp.LoadingPlaceId == bestRouteToAttend.InitialLoadingPlaceId));
                    bestRouteToAttend.ReturnTime.AddMinutes(travelReturnTime);
                }

                deliveries.Remove(delivery);
                noScheduled--;
            }
        }

        public TruckMixer FindBestTruckMixerToAttendDelivery(List<LoadingPlace> loadingPlaces)
        {
            return new TruckMixer();
        }

        public Route FindBestRouteToAttendDelivery(List<LoadingPlace> loadingPlaces)
        {
            return new Route();
        }

        public int NearestPossibleDepot(DeliveryOrder order, List<LoadingPlace> loadingPlaces)
        {
            return 0;
        }

        public List<LoadingPlace> GetLoadingPlaces()
        {
            return new List<LoadingPlace>();
        }

        public List<TruckMixer> GetTruckMixers()
        {
            return new List<TruckMixer>();
        }

        public List<DeliveryOrder> GetOrders()
        {
            return new List<DeliveryOrder>();
        }
    }
}
