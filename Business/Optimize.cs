using Business.Extensions;
using Entity;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    public class Optimize
    {
        public void Execute(int instanceNumber, DateTime begin, DateTime end, DeliveryOrder newDeliveryOrder)
        {
            List<LoadingPlace> loadingPlaces = GetLoadingPlaces(instanceNumber, begin, end);
            List<Delivery> deliveries = GetDeliveries(instanceNumber, begin, end, newDeliveryOrder);

            int noScheduled = deliveries.Count;
            while(noScheduled > 0)
            {
                Delivery delivery = deliveries.FirstOrDefault();

                Route bestRouteToAttend = FindBestRouteToAttendDelivery(loadingPlaces, delivery);

                if(bestRouteToAttend == null)
                {
                    TruckMixer truckMixer = FindBestTruckMixerToAttendDelivery(loadingPlaces);
                    int tripDurationTime = delivery.GetTravelTime(
                        loadingPlaces.FirstOrDefault(lp => lp.LoadingPlaceId == bestRouteToAttend.InitialLoadingPlaceId));
                    DateTime endServiceTimeAtCustomer = 
                        delivery.ServiceTimeBegin.AddMinutes(delivery.CustomerRmcDischargeRate * delivery.Volume);
                    Route newRoute = new Route()
                    {
                        TruckMixerId = truckMixer.TruckMixerId,
                        RmcType = delivery.RmcType,
                        TotalVolume = delivery.Volume,
                        InitialLoadingPlaceId = truckMixer.InitialLoadingPlaceId,
                        FinalLoadingPlaceId = truckMixer.InitialLoadingPlaceId,
                        LoadingEnd = delivery.ServiceTimeBegin.AddMinutes(-1 * tripDurationTime),
                        LoadingBegin = delivery.ServiceTimeBegin.AddMinutes(-1 * (tripDurationTime + delivery.Volume)),
                        RouteNodes = new List<RouteNode>() 
                        { 
                            new RouteNode() 
                            {
                                DeliveryOrderId = delivery.DeliveryOrderId,
                                DeliveryId = delivery.DeliveryId,
                                BeginService = delivery.ServiceTimeBegin,
                                EndService = endServiceTimeAtCustomer
                            } 
                        },
                        ReturnTime = endServiceTimeAtCustomer.AddMinutes(tripDurationTime * 0.8)
                    };
                }
                else
                {
                    bestRouteToAttend.TotalVolume += delivery.Volume;
                    bestRouteToAttend.LoadingEnd.AddMinutes(delivery.Volume);
                    DateTime endServiceTimeAtCustomer =
                        delivery.ServiceTimeBegin.AddMinutes(delivery.CustomerRmcDischargeRate * delivery.Volume);
                    bestRouteToAttend.RouteNodes.Add(new RouteNode()
                    {
                        RouteId = bestRouteToAttend.RouteId,
                        BeginService = delivery.ServiceTimeBegin,
                        EndService = endServiceTimeAtCustomer
                    });
                    int travelReturnTime = delivery.GetTravelTime(
                        loadingPlaces.FirstOrDefault(lp => lp.LoadingPlaceId == bestRouteToAttend.InitialLoadingPlaceId));
                    bestRouteToAttend.ReturnTime = endServiceTimeAtCustomer.AddMinutes(travelReturnTime * 0.8);
                }

                deliveries.Remove(delivery);
                noScheduled--;
            }
        }

        public TruckMixer FindBestTruckMixerToAttendDelivery(List<LoadingPlace> loadingPlaces)
        {
            return new TruckMixer();
        }

        public Route FindBestRouteToAttendDelivery(List<LoadingPlace> loadingPlaces, Delivery delivery)
        {
            return new Route();
        }

        public int NearestPossibleDepot(DeliveryOrder order, List<LoadingPlace> loadingPlaces)
        {
            return 0;
        }

        public List<LoadingPlace> GetLoadingPlaces(int instanceNumber, DateTime begin, DateTime end)
        {
            List<LoadingPlace> loadingPlaces = LoadingPlaceRepository.GetLoadingPlaces(instanceNumber, begin, end);
            return loadingPlaces;
        }

        public List<Delivery> GetDeliveries(int instanceNumber, DateTime begin, DateTime end, 
            DeliveryOrder newDeliveryOrder)
        {
            List<Delivery> oldDeliveries = 
                DeliveryOrderRepository.GetDeliveriesOrdersWithDeliveryOrderTrips(instanceNumber, begin, end);
            List<Delivery> newDeliveries = newDeliveryOrder.GetDeliveries();
            List<Delivery> deliveries = new List<Delivery>();
            deliveries.AddRange(newDeliveries);
            deliveries.AddRange(oldDeliveries);
            return deliveries.OrderBy(d => d.ServiceTimeBegin).ToList();
        }

        public Optimize(ILoadingPlaceRepository _LoadingPlaceRepository, 
            IDeliveryOrderRepository _DeliveryOrderRepository)
        {
            LoadingPlaceRepository = _LoadingPlaceRepository;
            DeliveryOrderRepository = _DeliveryOrderRepository;
        }

        ILoadingPlaceRepository LoadingPlaceRepository { get; set; }
        IDeliveryOrderRepository DeliveryOrderRepository { get; set; }
    }
}
