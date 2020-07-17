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

        private struct PossibleRoute
        {
            public int LoadingPlaceId { get; set; }
            public int TruckMixerId { get; set; }
            public int RouteId { get; set; }
            public int MinutesToTravel { get; set; }
            public double DistancePrecedingNode { get; set; }
        };

        public Route FindBestRouteToAttendDelivery(List<LoadingPlace> loadingPlaces, Delivery delivery)
        {
            double minDistance = double.MaxValue;
            int loadingPlaceMinDistanceId = 0;
            PossibleRoute possibleRoute = new PossibleRoute();

            foreach (var loadingPlace in loadingPlaces)
            {
                int currentDistancePrecedingNode = int.MaxValue;
                foreach(var truckMixer in loadingPlace.TruckMixers)
                {
                    foreach(var route in truckMixer.Routes)
                    {
                        if(route.TotalVolume + delivery.Volume <= truckMixer.Capacity)
                        {
                            RouteNode lastRouteNode = route.RouteNodes.Last();
                            double distancePrecedingNode = delivery.GeoCoordinates.GetDistanceTo(lastRouteNode.GeoCordinates);
                            int minutesToTravel = 2 * (int)distancePrecedingNode;

                            if (distancePrecedingNode < currentDistancePrecedingNode &&
                                lastRouteNode.EndService.AddMinutes(minutesToTravel) <= delivery.ServiceTimeBegin)
                            {
                                if(delivery.ServiceTimeBegin.Subtract(lastRouteNode.EndService.AddMinutes(minutesToTravel)).TotalMinutes <= 60)
                                {
                                    possibleRoute.LoadingPlaceId = loadingPlace.LoadingPlaceId;
                                    possibleRoute.TruckMixerId = truckMixer.TruckMixerId;
                                    possibleRoute.RouteId = route.RouteId;
                                    possibleRoute.MinutesToTravel = minutesToTravel;
                                    possibleRoute.DistancePrecedingNode = distancePrecedingNode;
                                }
                            }
                        }
                    }
                }

                double distanceCurrentLoadingPlace = delivery.GeoCoordinates.GetDistanceTo(loadingPlace.GeoCordinates);
                if(distanceCurrentLoadingPlace < minDistance)
                {
                    loadingPlaceMinDistanceId = loadingPlace.LoadingPlaceId;
                    minDistance = distanceCurrentLoadingPlace;
                }

            }
            
            if(possibleRoute.DistancePrecedingNode <= minDistance)
            {
                LoadingPlace loadingPlacePossibleSelected = loadingPlaces.FirstOrDefault(l => l.LoadingPlaceId == possibleRoute.LoadingPlaceId);
                TruckMixer truckMixerPossibleSelected = loadingPlacePossibleSelected.TruckMixers.FirstOrDefault(tm => tm.TruckMixerId == possibleRoute.TruckMixerId);
                Route routePossibleSelected = truckMixerPossibleSelected.Routes.FirstOrDefault(r => r.RouteId == possibleRoute.RouteId);
                routePossibleSelected.RouteNodes.Add(new RouteNode() 
                { 
                    RouteNodeId = routePossibleSelected.NextRouteNodeId,
                    RouteId = routePossibleSelected.RouteId,
                    DeliveryOrderId = delivery.DeliveryOrderId,
                    DeliveryId = delivery.DeliveryId,
                    BeginService = delivery.ServiceTimeBegin,
                    EndService = delivery.ServiceTimeBegin.AddMinutes(delivery.CustomerRmcDischargeRate * delivery.Volume),
                    Longitude = delivery.Longitude,
                    Latitude = delivery.Latitude
                });
                routePossibleSelected.NextRouteNodeId++;
                truckMixerPossibleSelected.EndLastService = delivery.ServiceTimeBegin.
                    AddMinutes(delivery.CustomerRmcDischargeRate * delivery.Volume).
                    AddMinutes(2 * delivery.GeoCoordinates.GetDistanceTo(loadingPlacePossibleSelected.GeoCordinates));
            }
            else
            {
                LoadingPlace loadingPlaceMinDistanceSelected = loadingPlaces.FirstOrDefault(l => l.LoadingPlaceId == loadingPlaceMinDistanceId);

            }

            return new Route();
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
