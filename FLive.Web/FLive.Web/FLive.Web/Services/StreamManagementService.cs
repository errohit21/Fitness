using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Repositories;
using FLive.Web.Shared;
using FLive.Web.Shared.Wowza;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FLive.Web.Services
{
    public class StreamManagementService : IStreamManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly WowzaConfig _wowzaConfig;
       

        public StreamManagementService( IOptions<WowzaConfig> settings)
        {
            // _context = context;
            _wowzaConfig = settings.Value;

            _httpClient = new HttpClient();
            ConfigureHttpClient();
        }

        public async Task<string> Create( CreateStreamViewModel createStreamViewModel)
        {
            var wowzaRegion = GetWowzaRegion(createStreamViewModel.WowzaRegion);
            var payload = BuildCreateStreamContent(Guid.NewGuid().ToString(), wowzaRegion);
            // name : username+date region:closest to the user
            var result = await _httpClient.PostAsync("live_streams", payload);
            var jsonResult = await result.Content.ReadAsStringAsync();
            dynamic d = JObject.Parse(jsonResult);
            
            return d.live_stream.id;
        }


        private string GetWowzaRegion(string location)
        {
            switch (location)
            {
                case "India":
                    return "asia_pacific_india";
                case "Australia":
                    return "asia_pacific_australia";
                case "WestUS":
                    return "us_west_california";
                case "EastUS":
                default:
                    return "asia_pacific_australia";
            }
        }
        public async Task<string> Get(string streamId)
        {
            var result = await _httpClient.GetAsync($"live_streams/{streamId}");
            return await result.Content.ReadAsStringAsync();
        }

        public async Task<string> Start(string streamId)
        {
            var result = await _httpClient.PutAsync($"live_streams/{streamId}/start", new StringContent(""));
            return await result.Content.ReadAsStringAsync();
        }

        public async Task<string> Stop(string streamId)
        {
            var result = await _httpClient.PutAsync($"live_streams/{streamId}/stop", new StringContent(""));
            return await result.Content.ReadAsStringAsync();
        }

        public async Task<string> Delete(string streamId)
        {
            var result = await _httpClient.DeleteAsync($"live_streams/{streamId}");
            return await result.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAll()
        {
            var result = await _httpClient.GetAsync("live_streams");
            return await result.Content.ReadAsStringAsync();
        }

      

        private void ConfigureHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Add("wsc-api-key", _wowzaConfig.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("wsc-access-key", _wowzaConfig.AccessKey);
            _httpClient.BaseAddress = new Uri($"https://{_wowzaConfig.Endpoint}/api/v1/");
        }

        private StringContent BuildCreateStreamContent(string name, string broadcastLocation)
        {
            //var payload = new StringContent("{\"live_stream\": {\"name\": \"MyNewLiveStream2\", \"transcoder_type\": \"transcoded\", \"billing_mode\": \"pay_as_you_go\", \"broadcast_location\": \"us_west_california\", \"encoder\": \"other_rtmp\", \"delivery_method\": \"push\", \"aspect_ratio_width\": 1920, \"aspect_ratio_height\": 1080}}", Encoding.UTF8, "application/json");

            var stream = new live_stream { name = name, broadcast_location = broadcastLocation ,  };
            var liveStreamJson = JsonConvert.SerializeObject(stream);

            var payload = new StringContent("{\"live_stream\": " + liveStreamJson + "}", Encoding.UTF8,
                "application/json");
            return payload;
        }
    }
}