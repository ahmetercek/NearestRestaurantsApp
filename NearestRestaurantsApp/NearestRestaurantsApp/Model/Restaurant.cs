namespace NearestRestaurantsApp.Model
{
    public class Restaurant
    {
        public string Name { get; set; }
        public Position Location { get; set; }
        public string Adress { get; set; }
        public string AdressDetail { get; set; }
        public string Phone { get; set; }
        public double Ratings { get; set; }
    }
}
