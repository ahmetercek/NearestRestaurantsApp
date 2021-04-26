using Android.Content;
using Android.Util;
using Huawei.Hms.Site.Api;
using Huawei.Hms.Site.Api.Model;
using NearestRestaurantsApp.Droid.Services;
using NearestRestaurantsApp.Model;
using NearestRestaurantsApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(RestaurantSearchService))]
namespace NearestRestaurantsApp.Droid.Services
{
    public class RestaurantSearchService : Java.Lang.Object, ISearchResultListener, IRestaurantSearchService
    {
        readonly string Tag = "RestaurantSearchService";

        ISearchService _searchService;
        IList<Restaurant> restaurants;
        TaskCompletionSource<bool> tcsResult;

        public async Task<IList<Restaurant>> GetRestaurantsAsync(Position position)
        {          
            tcsResult = new TaskCompletionSource<bool>();

            _searchService = SearchServiceFactory.Create(Android.App.Application.Context, Android.Net.Uri.Encode("YOUR_API_KEY"));
            
            // Create SearchRequest
            NearbySearchRequest nearbySearchRequest = new NearbySearchRequest();
            nearbySearchRequest.Query = "Istanbul";
            nearbySearchRequest.Language = "en";
            nearbySearchRequest.PoliticalView = "TR";
            // Set Poi type as Restaurant
            nearbySearchRequest.PoiType = LocationType.Restaurant;
            //Set user's location for nearest restaurant search
            nearbySearchRequest.Location = new Coordinate(position.Latitude, position.Longitude);
            nearbySearchRequest.Radius = Java.Lang.Integer.ValueOf(10000);
            nearbySearchRequest.PageIndex = Java.Lang.Integer.ValueOf(1);
            nearbySearchRequest.PageSize = Java.Lang.Integer.ValueOf(10);

            // Call the nearby place search API.
            _searchService.NearbySearch(nearbySearchRequest, this);

            await tcsResult.Task;

            return restaurants;
        }

        /// <summary>
        /// Obtains the search result.
        /// </summary>
        public void OnSearchResult(Java.Lang.Object results)
        {
            restaurants = new List<Restaurant>();
            NearbySearchResponse nearbySearchResponse = (NearbySearchResponse)results;
           
            foreach (Site site in nearbySearchResponse.Sites)
            {
                restaurants.Add(new Restaurant(){ 
                    Name = site.Name,
                    Location = new Position(site.Location.Lat, site.Location.Lng),
                    Adress = site.Address.AdminArea,
                    AdressDetail = site.Address.Locality + " " + site.Address.SubLocality + " " + site.Address.Thoroughfare,
                    Phone = site.Poi.Phone,
                    Ratings = site.Poi.Rating
                });
            }

            tcsResult.TrySetResult(true);
        }

        /// <summary>
        /// Obtains the search error status.
        /// </summary>
        public void OnSearchError(SearchStatus status)
        {
            Log.Debug(Tag, "Error Code: " + status.ErrorCode + " Error Message: " + status.ErrorMessage);
        }
    }
}