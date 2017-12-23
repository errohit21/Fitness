using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FLive.Infrastructure
{
    public class BaseService
    {
        private readonly string _apiEndpoint;
        private readonly string _apiToken;
        public BaseService(string apiEndpoint, string apiToken)
        {
            _apiEndpoint = apiEndpoint;
            _apiToken = apiToken;
        }
        protected HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_apiEndpoint);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiToken}");
            return httpClient;
        }


        protected static async Task<string> SendReminderNotification(HttpClient httpClient, long liveworkoutId)
        {
            var viewModel = BuildReminderViewModel(liveworkoutId);
            var result =
                await
                    httpClient.PostAsync("api/notifications/liveworkoutreminder",
                        new StringContent(viewModel, Encoding.UTF8, "application/json"));
            var jsonResult = await result.Content.ReadAsStringAsync();
            return jsonResult;
        }

        protected static string BuildReminderViewModel(long liveWorkoutId)
        {
            return $@"{{""LiveWorkoutId"":""{liveWorkoutId}""}}";
        }

    }
}