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
    public class WorkoutStartNotifier : BaseService
    {
       
        private readonly string _dbConnection;

        public WorkoutStartNotifier(string dbConnection, string apiEndpoint, string apiToken) : base(apiEndpoint,apiToken)
        {
            _dbConnection = dbConnection;
          
        }

        public async Task SendReminders()
        {
            var successful = true;


            using (var connection = new SqlConnection(_dbConnection))
            {
                connection.Open();
                await SendAtInterval(connection,30);
                await SendAtInterval(connection, 15);
            }
        }

        private async Task SendAtInterval(SqlConnection connection , int interval)
        {
            var lowerLimit = DateTime.UtcNow.AddMinutes((interval - 5)).ToString("o");
            var upperLimit = DateTime.UtcNow.AddMinutes(interval).ToString("o");

            var items =
                connection.Query<UpcomingLiveWorkout>(
                    $"select * from  LiveWorkouts WHERE StartTime between  '{lowerLimit}' and '{upperLimit}'");

            Console.WriteLine(
                $"Found {items.Count()} items between {lowerLimit} and {upperLimit} liveworkouts to create");
            var httpClient = GetHttpClient();
            foreach (var item in items.ToList())
            {
                Console.WriteLine($"Sending {interval} Mins reminder notification for workout : {item.Id}.");
                await SendReminderNotification(httpClient, item.Id);

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

        private static string BuildReminderViewModel(long liveWorkoutId)
        {
            return $@"{{""LiveWorkoutId"":""{liveWorkoutId}""}}";
        }

    }
}