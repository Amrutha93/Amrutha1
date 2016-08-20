using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFAppBingMapService.GeocodeService;
using WPFAppBingMapService.ImageryService;

namespace WPFAppBingMapService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Geocode method
        private String GeocodeAddress(string address)
        {
            string results = "";
            string key = "AogJ6dCrh65ddcr1hkRuGUNDblRCi3j5qnbChQ0eG6rJqtRhEdM0VAFn2av4VY1I";

            GeocodeRequest geocodeRequest = new GeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            geocodeRequest.Credentials = new WPFAppBingMapService.GeocodeService.Credentials();
            geocodeRequest.Credentials.ApplicationId = key;

            // Set the full address query
            geocodeRequest.Query = address;

            // Set the options to only return high confidence results 
            ConfidenceFilter[] filters = new ConfidenceFilter[1];
            filters[0] = new ConfidenceFilter();
            filters[0].MinimumConfidence = WPFAppBingMapService.GeocodeService.Confidence.High;

            // Add the filters to the options
            GeocodeOptions geocodeOptions = new GeocodeOptions();
            geocodeOptions.Filters = filters;
            geocodeRequest.Options = geocodeOptions;

            // Make the geocode request
            GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

            if (geocodeResponse.Results.Length > 0)
                results = String.Format("{0}, {1}",
                  geocodeResponse.Results[0].Locations[0].Latitude,
                              geocodeResponse.Results[0].Locations[0].Longitude);
            else
                results = "No Results Found";

            return results;
        }

        //Imagery Method
        private string GetImagery(string locationString)
        {
            string key = "AogJ6dCrh65ddcr1hkRuGUNDblRCi3j5qnbChQ0eG6rJqtRhEdM0VAFn2av4VY1I";
            MapUriRequest mapUriRequest = new MapUriRequest();

            // Set credentials using a valid Bing Maps key
            mapUriRequest.Credentials = new WPFAppBingMapService.ImageryService.Credentials();
            mapUriRequest.Credentials.ApplicationId = key;

            // Set the location of the requested image
            mapUriRequest.Center = new WPFAppBingMapService.ImageryService.Location();
            string[] digits = locationString.Split(',');
            mapUriRequest.Center.Latitude = double.Parse(digits[0].Trim());
            mapUriRequest.Center.Longitude = double.Parse(digits[1].Trim());

            // Set the map style and zoom level
            MapUriOptions mapUriOptions = new MapUriOptions();
            mapUriOptions.Style = MapStyle.AerialWithLabels;
            mapUriOptions.ZoomLevel = GetMapZoomLevel();

            // Set the size of the requested image in pixels
            mapUriOptions.ImageSize = new WPFAppBingMapService.ImageryService.SizeOfint();
            mapUriOptions.ImageSize.Height = 200;
            mapUriOptions.ImageSize.Width = 300;

            mapUriRequest.Options = mapUriOptions;

            //Make the request and return the URI
            ImageryServiceClient imageryService = new ImageryServiceClient("BasicHttpBinding_IImageryService");
            MapUriResponse mapUriResponse = imageryService.GetMapUri(mapUriRequest);
            return mapUriResponse.Uri;
        }



        private void RequestImage_Click(object sender, RoutedEventArgs e)
        {
            labelResults.Content = GeocodeAddress(textInput.Text);
            // Make label hidden and image visible
            labelResults.Visibility = Visibility.Hidden;
            ImageResults.Visibility = Visibility.Visible;

            //Get URI and set image
            String imageURI = GetImagery(labelResults.Content.ToString());
            ImageResults.Source = new BitmapImage(new Uri(imageURI));

        }

        private void mapSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GetMapZoomLevel();
        }
        private int GetMapZoomLevel()
        {
            int width;
            // Check the Window1.xaml file to make sure the mapSlider value is not 0;
            width = Convert.ToInt32(mapSlider.Value);
            return width;
        }


    }
}
