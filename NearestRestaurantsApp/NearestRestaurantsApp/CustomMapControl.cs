using NearestRestaurantsApp.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace NearestRestaurantsApp
{
    public class CustomMapControl : View
    {

        #region BindableProperties
        public static readonly BindableProperty CustomPinsProperty = BindableProperty.Create(nameof(CustomPins), typeof(IList<Restaurant>), typeof(CustomMapControl), default(IList<Restaurant>));

        #endregion

        #region Properties

        public IList<Restaurant> CustomPins
        {
            get { return (IList<Restaurant>)GetValue(CustomPinsProperty); }
            set { SetValue(CustomPinsProperty, value); }
        }
        #endregion
    }
}
