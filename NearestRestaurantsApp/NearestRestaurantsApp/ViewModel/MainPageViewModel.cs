using NearestRestaurantsApp.Model;
using NearestRestaurantsApp.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NearestRestaurantsApp.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public const string Tag = nameof(MainPageViewModel);

        private ILocationService _locationService;
        private IRestaurantSearchService _restaurantSearchService;
        private ILoggerService _loggerService;

        private ObservableCollection<Restaurant> _restaurants;
        public ObservableCollection<Restaurant> Restaurants
        {
            get 
            { 
                return _restaurants; 
            }
            set 
            {
                if(_restaurants != value)
                {
                    _restaurants = value;
                    OnPropertyChanged("Restaurants");
                }               
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            this._locationService = DependencyService.Get<ILocationService>();
            this._restaurantSearchService = DependencyService.Get<IRestaurantSearchService>();
            this._loggerService = DependencyService.Get<ILoggerService>();

            AttachPinsToMap();
        }

        public async void AttachPinsToMap()
        {
            Position userLocation = await ObtainUserLocation();
            ObtainRestaurants(userLocation);
        }

        public async void ObtainRestaurants(Position position)
        {
            var restaurantSearchTask = _restaurantSearchService.GetRestaurantsAsync(position);
            try
            {
                await restaurantSearchTask;
                if (restaurantSearchTask.Result != null)
                {
                    Restaurants = new ObservableCollection<Restaurant>(restaurantSearchTask.Result);
                }
            }
            catch (Exception e)
            {
                _loggerService.Log(Tag, $"ObtainRestaurants error: {e.Message}");
            }
        }

        public async Task<Position> ObtainUserLocation()
        {
            var status = await CheckAndRequestLocationPermission();
            if (status != PermissionStatus.Granted)
            {
                // User permission was denied
                return null;
            }

            var locationTask = _locationService.GetLocationAsync();
            try
            {
                await locationTask;
                if (locationTask.Result != null)
                {
                    return locationTask.Result;
                }
            }
            catch (Exception e)
            {
                _loggerService.Log(Tag, $"ObtainUserLocation error: {e.Message}");
            }

            return null;
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
