using NearestRestaurantsApp.Model;
using System.Threading.Tasks;

namespace NearestRestaurantsApp.Services
{
    public interface ILocationService
    {
        Task<Position> GetLocationAsync();
    }
}
