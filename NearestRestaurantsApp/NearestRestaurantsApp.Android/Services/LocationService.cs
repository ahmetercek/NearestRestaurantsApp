using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Util;
using Huawei.Hms.Location;
using NearestRestaurantsApp.Droid.Services;
using NearestRestaurantsApp.Model;
using NearestRestaurantsApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationService))]
namespace NearestRestaurantsApp.Droid.Services
{
    public class LocationService : LocationCallback, ILocationService 
    {
        readonly string Tag = "LocationService";

        FusedLocationProviderClient fusedLocationProviderClient;
        Position userLocation;
        TaskCompletionSource<bool> tcsResult;

        public async Task<Position> GetLocationAsync()
        {
            tcsResult = new TaskCompletionSource<bool>();
            Context context = Android.App.Application.Context;

            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(context);            
            LocationRequest locationRequest = new LocationRequest();
            //Set the location update interval (int milliseconds).        
            locationRequest.SetInterval(10000);
            //Set the weight.        
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);

            try
            {
                await fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, this, Looper.MainLooper);
                await tcsResult.Task;
                Log.Info(Tag, $"User Location {userLocation.Longitude},{userLocation.Latitude}");
                return userLocation;
            }
            catch(Exception e)
            {
                Log.Error(Tag, $"GetLocationAsync exception: {e.Message}");
            }

            return null;
        }

        /// <summary>
        /// Called when the device location is available.
        /// </summary>
        public override void OnLocationResult(LocationResult locationResult)
        {
            if (locationResult != null)
            {
                IList<Location> locations = locationResult.Locations;
                if (locations.Count != 0)
                    userLocation = new Position(locations[0].Latitude, locations[0].Longitude);
                tcsResult.TrySetResult(true);
            }
        }

        /// <summary>
        /// Called when the device location availability changed.
        /// </summary>
        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
                Log.Info(Tag, "OnLocationAvailability");           
        }

    }
}