using Android.Content;
using Android.OS;
using Android.Widget;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;
using NearestRestaurantsApp;
using NearestRestaurantsApp.Droid.CustomRenderer;
using NearestRestaurantsApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMapControl), typeof(CustomMapRenderer))]
namespace NearestRestaurantsApp.Droid.CustomRenderer
{

    public class CustomMapRenderer : ViewRenderer<CustomMapControl, MapView> , IOnMapReadyCallback, HuaweiMap.IInfoWindowAdapter
    {
        protected MapView NativeMap => Control;
        protected CustomMapControl Map => Element;

        static Bundle s_bundle;
        internal static Bundle Bundle
        {
            set { s_bundle = value; }
        }

        IList<Restaurant> customPins;
        HuaweiMap hMap;

        public CustomMapRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomMapControl> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var mapView = new MapView(Context);
                mapView.OnCreate(s_bundle);
                mapView.OnResume();
                SetNativeControl(mapView);
            }

            if (e.OldElement != null)
            {
                MapView oldMapView = Control;
                oldMapView.OnDestroy();
            }

            if (e.NewElement != null)
            {
                Control.GetMapAsync(this);

            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnElementPropertyChanged: " + e.PropertyName);
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomMapControl.CustomPinsProperty.PropertyName)
            {
                customPins = Map.CustomPins;
                AttachPinsToMap();
            }
        }

        public void OnMapReady(HuaweiMap hMap)
        {
            if (hMap == null)
            {
                return;
            }
            this.hMap = hMap;
            hMap.SetInfoWindowAdapter(this);
        }

        public void AttachPinsToMap()
        {
            if(customPins != null)
            {
                foreach (Restaurant restaurant in customPins)
                {
                    Marker marker;
                    MarkerOptions markerOptions = new MarkerOptions()
                        .InvokePosition(new LatLng(restaurant.Location.Latitude, restaurant.Location.Longitude))
                        .InvokeTitle(restaurant.Name)
                        .InvokeSnippet(restaurant.Adress);

                    marker = hMap.AddMarker(markerOptions);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (NativeMap != null)
                {
                    NativeMap.Dispose();
                }

                Control?.OnDestroy();
            }
            
            System.Diagnostics.Debug.WriteLine("Disposing: " + disposing);
            base.Dispose(disposing);
        }

        /// <summary>
        /// If this method returns null, the default information window will be displayed.
        /// </summary>
        /// <param name="marker">Marker for which an information window needs to be displayed.</param>
        /// <returns>View object</returns>
        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null;
        }

        /// <summary>
        /// Provides a custom information window view for a marker.
        /// If this method returns a view, the view will be used for the entire information window.
        /// </summary>
        /// <param name="marker">Marker for which an information window needs to be displayed.</param>
        /// <returns>View object.</returns>
        public Android.Views.View GetInfoWindow(Marker marker)
        {
            if (marker == null)
                return null;

            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                //Get restaurant by marker.
                Restaurant restaurant = GetCustomPin(marker);

                view = inflater.Inflate(Resource.Layout.CustomMapInfoWindow, null);

                //Bind view components
                TextView textviewTitle = view.FindViewById<TextView>(Resource.Id.customInfoTitle);
                TextView textviewDescription = view.FindViewById<TextView>(Resource.Id.customInfoDescription);
                TextView textviewRestaurantAdress = view.FindViewById<TextView>(Resource.Id.restaurantAdress);
                RatingBar ratingBar = view.FindViewById<RatingBar>(Resource.Id.customInfoRatingBar);
                Android.Widget.Button reservationButton = view.FindViewById<Android.Widget.Button>(Resource.Id.btn_reservation);
                
                //Set InfoWindow values
                textviewTitle.Text = restaurant.Name;
                textviewDescription.Text = restaurant.Adress;
                textviewRestaurantAdress.Text = restaurant.AdressDetail;
                ratingBar.Rating = restaurant.Ratings !=0 ? (float)restaurant.Ratings : 4;

                //Open Phone Dialer when click button.
                reservationButton.Click += delegate { PlacePhoneCall(restaurant.Phone); };

                return view;
            }
            return null;
        }

        /// <summary>
        /// Obtains Restaurant object
        /// of selected marker by position.
        /// </summary>
        /// <param name="annotation">Selected marker</param>
        /// <returns>Restaurant object</returns>
        Restaurant GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Location.Latitude == position.Latitude && pin.Location.Longitude == position.Longitude)
                {
                    return pin;
                }
            }
            return null;
        }

        /// <summary>
        /// Opens Phone Dialer.
        /// </summary>
        /// <param name="number">Restaurant phone number</param>
        public void PlacePhoneCall(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}