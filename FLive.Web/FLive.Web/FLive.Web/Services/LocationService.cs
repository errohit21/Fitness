using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Services.Interfaces;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FLive.Web.Services
{
    public class LocationService : ILocationService
    {
        public async Task<int> GetPostCodeByLocation(LocationDataViewModel locationDataViewModel)
        {

            try
            {
                var httpClient = GetLocationHttpClient();
                var url =
                    $"https://maps.google.com/maps/api/geocode/json?latlng={locationDataViewModel.Latitude},{locationDataViewModel.Longitude}&sensor=false&key=AIzaSyD8pzQypkAxfn7r0bh2FUONyc92nos5ZIY";
                var result = await httpClient.GetAsync(url);

                var jsonResult = await result.Content.ReadAsStringAsync();

                Rootobject obj = JsonConvert.DeserializeObject<Rootobject>(jsonResult);

                var postcodeContainer = obj.results.FirstOrDefault(a => a.types.Contains("postal_code"));
                var postcodeComponent = postcodeContainer.address_components.FirstOrDefault(a => a.types.Contains("postal_code"));

                var postcode = postcodeComponent.short_name.AsInt();
                return await Task.FromResult(postcode);
            }
            catch
            {
                return await Task.FromResult(4000); ;
            }
        }

        private HttpClient GetLocationHttpClient()
        {
            return new HttpClient ();
        }


    }


    public class Rootobject
    {
        public Result[] results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public Address_Components[] address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
        public Bounds bounds { get; set; }
    }

    public class Location
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Bounds
    {
        public Northeast1 northeast { get; set; }
        public Southwest1 southwest { get; set; }
    }

    public class Northeast1
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest1
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Address_Components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

}