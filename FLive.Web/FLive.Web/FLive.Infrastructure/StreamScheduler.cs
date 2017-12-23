using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FLive.Infrastructure.Models;
using Serilog;

namespace FLive.Infrastructure
{
    public class StreamScheduler  :BaseService
    {
       
        private readonly string _dbConnection;

        public StreamScheduler(string dbConnection, string apiEndpoint, string apiToken) : base(apiEndpoint,apiToken)
        {
            _dbConnection = dbConnection;
          
        }

        public async Task CreateAndStart()
        {
            var successful = true;


            using (var connection = new SqlConnection(_dbConnection))
            {
                connection.Open();

                var lowerLimit = DateTime.UtcNow.ToString("o");
                var upperLimit = DateTime.UtcNow.AddMinutes(10).ToString("o");

                var items =
                    connection.Query<UpcomingLiveWorkout>(
                        $"select * from  UpcomingLiveWorkouts WHERE StartTime between  '{lowerLimit}' and '{upperLimit}' and WorkoutType=1");

                Console.WriteLine(
                    $"Found {items.Count()} items between {lowerLimit} and {upperLimit} liveworkouts to create");
                var httpClient = GetHttpClient();
                foreach (var item in items.ToList())
                {
                    Console.WriteLine($"Creating stream for live workout: {item.Id}.");

                    var streamId = await CreateStream(httpClient, item.CreateStreamJson);


                    if (string.IsNullOrEmpty(streamId))
                    {
                        Log.Logger.Error($"Failed to create the stream for LiveWorkout : {item.Id}.");
                        Console.Error.WriteLine($"Failed to create the stream for LiveWorkout : {item.Id}.");
                    }
                    else
                    {
                        Console.WriteLine($"Starting stream:  {streamId} for live workout : {item.Id}.");
                        var startResult = await StartStream(streamId, httpClient);

                        Console.WriteLine(
                            $"Updating table LiveWorkouts with stream: {streamId} for live workout {item.Id}.");

                        connection.Execute(
                            $"update LiveWorkouts set WorkoutStatus=2 , StreamId='{streamId}' where Id={item.LiveWorkoutId}");

                        connection.Execute($"delete UpcomingLiveWorkouts where Id={item.Id}");

                        Console.WriteLine($"Sending reminder notification for workout : {item.Id}.");

                        await SendReminderNotification(httpClient, item.LiveWorkoutId);
                    }
                }
            }
        }

        private static async Task<string> CreateStream(HttpClient httpClient, string createStreamJson)
        {
            var result =
                await
                    httpClient.PostAsync("api/stream/create",
                        new StringContent(createStreamJson, Encoding.UTF8, "application/json"));
            var jsonResult = await result.Content.ReadAsStringAsync();


            return jsonResult;
        }

       
        private async Task<bool> StartStream(string streamId, HttpClient httpClient)
        {
            var result =
                await
                    GetHttpClient()
                        .PostAsync($"api/stream/start/{streamId}",
                            new StringContent("", Encoding.UTF8, "application/json"));
            return result.StatusCode == HttpStatusCode.OK;
        }

        private async Task<bool> StopStream(string streamId, HttpClient httpClient)
        {
            var result =
                await
                    GetHttpClient()
                        .PostAsync($"api/stream/stop/{streamId}",
                            new StringContent("", Encoding.UTF8, "application/json"));
            return result.StatusCode == HttpStatusCode.OK;
        }

        private async Task<bool> DelteStream(string streamId, HttpClient httpClient)
        {
            var result =
                await
                    GetHttpClient()
                        .PostAsync($"api/stream/delete/{streamId}",
                            new StringContent("", Encoding.UTF8, "application/json"));
            return result.StatusCode == HttpStatusCode.OK;
        }

       
        public async Task StopCompletedWorkouts()
        {
            using (var connection = new SqlConnection(_dbConnection))
            {
                connection.Open();

                var currentTime = DateTime.UtcNow.ToString("o");

                var httpClient = GetHttpClient();
                var items =
                    connection.Query<LiveWorkout>(
                        $"select * from  LiveWorkouts WHERE EndTime <  '{currentTime}' and WorkoutStatus < 4");

                foreach (var item in items)
                {
                    if (IsValidStream(item))
                    {
                        await StopStream(item.StreamId, httpClient);
                        connection.Execute($"update LiveWorkouts set WorkoutStatus=4  where Id={item.Id}");
                    }

                   
                }
            }
        }

        public async Task Delete()
        {
            using (var connection = new SqlConnection(_dbConnection))
            {
                connection.Open();

                var timeToKeepStream = int.Parse(ConfigurationManager.AppSettings["TimeToRetainStreamInMins"]);
                var timeToKeepTheStream = DateTime.UtcNow.AddMinutes(-timeToKeepStream).ToString("o");//60 mins 

                var httpClient = GetHttpClient();
                var items =
                    connection.Query<LiveWorkout>(
                        $"select * from  LiveWorkouts WHERE EndTime <  '{timeToKeepTheStream}' or WorkoutStatus = 5");

                foreach (var item in items)
                {
                    if (IsValidStream(item))
                    {
                        await DelteStream(item.StreamId, httpClient);
                        connection.Execute($"update LiveWorkouts set WorkoutStatus=6 , StreamId='{item.StreamId}_archived' where Id={item.Id}");
                    }
                  
                }
            }
        }

        private static bool IsValidStream(LiveWorkout item)
        {
            return !string.IsNullOrEmpty(item.StreamId) && !item.StreamId.Contains("_archived");
        }
    }
}