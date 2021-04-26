using NearestRestaurantsApp.Droid.Services;
using NearestRestaurantsApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoggerService))]
namespace NearestRestaurantsApp.Droid.Services
{
    public class LoggerService : ILoggerService
    {
        public void Log(string tag, string message)
        {
            Android.Util.Log.Debug(tag, message);
        }
    }
}