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
    public class WorkoutPublisher : BaseService
    {
       
        private readonly string _dbConnection;

        public WorkoutPublisher(string dbConnection, string apiEndpoint, string apiToken) : base(apiEndpoint,apiToken)
        {
            _dbConnection = dbConnection;
          
        }

        public async Task PublishScheduledWorkouts()
        {
            var successful = true;

            using (var connection = new SqlConnection(_dbConnection))
            {
                connection.Open();

                var utcNow = DateTime.UtcNow.ToString("o");
                var items =
                    connection.Query<UpcomingLiveWorkout>(
                        $"select * from  UpcomingLiveWorkouts WHERE WorkoutType=2 and WorkoutPublishStatus=4 and (PublishDateTime <  '{utcNow}' or IsPublishImmediately=1)");//4=encodingcompleted

                Console.WriteLine(
                    $"Found {items.Count()} items between to be published");

                var httpClient = GetHttpClient();
                foreach (var item in items.ToList())
                {
                   
                        Console.WriteLine(
                            $"Updating table LiveWorkouts with publishedUrl: {item.MediaServiceUrl} for live workout {item.Id}.");

                        connection.Execute(
                            $"update LiveWorkouts set WorkoutStatus=5, recordingurl='{item.MediaServiceUrl}' where Id={item.LiveWorkoutId}");

                        connection.Execute($"delete UpcomingLiveWorkouts where Id={item.Id}");

                        Console.WriteLine($"Sending reminder notification for workout : {item.Id}.");

                        await SendReminderNotification(httpClient, item.LiveWorkoutId);
                    
                }
            }
        }


        private static async Task<string> SendReminderNotification(HttpClient httpClient, long liveworkoutId)
        {
            var viewModel = BuildReminderViewModel(liveworkoutId);
            var result =
                await
                    httpClient.PostAsync("api/notifications/liveworkoutreminder",
                        new StringContent(viewModel, Encoding.UTF8, "application/json"));
            var jsonResult = await result.Content.ReadAsStringAsync();
            return jsonResult;
        }

    }
}