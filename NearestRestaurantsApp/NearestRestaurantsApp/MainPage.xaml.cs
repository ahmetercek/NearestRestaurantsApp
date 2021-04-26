using NearestRestaurantsApp.ViewModel;
using Xamarin.Forms;

namespace NearestRestaurantsApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
    }
}
