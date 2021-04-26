using System;

namespace NearestRestaurantsApp.Model
{
    public class Position
    {
        public double Latitude { get; }

        public double Longitude { get; }

        public Position(double latitude, double longitude)
        {           
            Latitude = Math.Min(Math.Max(latitude, -90.0), 90.0);
            Longitude = Math.Min(Math.Max(longitude, -180.0), 180.0);
        }
    }
}
