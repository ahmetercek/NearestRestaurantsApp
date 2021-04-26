using NearestRestaurantsApp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NearestRestaurantsApp.Services
{
    public interface IRestaurantSearchService
    {
        Task<IList<Restaurant>> GetRestaurantsAsync(Position position);
    }
}
