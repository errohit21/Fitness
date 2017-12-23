using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.Slack;

namespace FLive.Infrastructure
{
    public class Program
    {
        public static void Main()
		{
			Console.WriteLine("Version 10.0.13");

			Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Warning()
              .Enrich.FromLogContext()
              .WriteTo.Slack("https://hooks.slack.com/services/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")
              .CreateLogger();

            Log.Logger.Information("Starting the background process @ {UtcNow} ", DateTime.UtcNow);

            Log.Logger.Information("Starting {}");
            var _dbConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            var _apiEndpoint = ConfigurationManager.AppSettings["apiEndpoint"];
            var _apiToken = ConfigurationManager.AppSettings["apiToken"];

            StreamScheduler streamScheduler = new StreamScheduler(_dbConnection, _apiEndpoint, _apiToken);
            WorkoutStartNotifier workoutStartNotifier = new WorkoutStartNotifier(_dbConnection, _apiEndpoint, _apiToken);
            PaymentsWorker paymentsWorker = new PaymentsWorker(_dbConnection, _apiEndpoint, _apiToken);
            DownloadService downloadService = new DownloadService(_dbConnection);
            WorkoutPublisher workoutPublisher = new WorkoutPublisher(_dbConnection, _apiEndpoint, _apiToken);
            
            Console.WriteLine("Checking for new streams to be created...");
            WrapInAExceptionHandlingBlock(() => streamScheduler.CreateAndStart()).Wait();
            Console.WriteLine("Finished creating new streams");

            
            Console.WriteLine("Checking for streams that neeeds to be stopped");
            WrapInAExceptionHandlingBlock(()=>streamScheduler.StopCompletedWorkouts()).Wait();
            Console.WriteLine("Finished stopping completed streams");

            Console.WriteLine("Checking for streams that neeeds to be stopped");
            WrapInAExceptionHandlingBlock(() => downloadService.DownloadRecordings()).Wait();
            Console.WriteLine("Finished stopping completed streams");

            Console.WriteLine("Checking for streams that neeeds to be deleted");
            WrapInAExceptionHandlingBlock(()=>streamScheduler.Delete()).Wait();
            Console.WriteLine("Finished deleteing streams that are more than 24 hours old");

            Console.WriteLine("Charging completed workouts");
            WrapInAExceptionHandlingBlock(()=>paymentsWorker.TryChargeCompletedWorkouts()).Wait();
            Console.WriteLine("Finished charging completed workouts");

            Console.WriteLine("Sending Reminder notifications");
            WrapInAExceptionHandlingBlock(() => workoutStartNotifier.SendReminders()).Wait();
            Console.WriteLine("Finished sending Reminder notifications");

            Console.WriteLine("Checking for new workouts to be published...");
            WrapInAExceptionHandlingBlock(() => workoutPublisher.PublishScheduledWorkouts()).Wait();
            Console.WriteLine("Finished publishing workouts");

            Log.Logger.Information("Finishing the background process @ {UtcNow} ", DateTime.UtcNow);

            // Console.ReadLine();
        }

        private static async Task WrapInAExceptionHandlingBlock(Func<Task> funciton)
        {
            try
            {
                await funciton.Invoke();
            }
            catch (Exception exception)
            {
                Log.Logger.Error("Exception in Infrastructure service {exception}", exception);

                Console.WriteLine(exception.Message);
                Console.Error.WriteLine(exception.Message , exception.StackTrace);
            }
        }
    }
}
