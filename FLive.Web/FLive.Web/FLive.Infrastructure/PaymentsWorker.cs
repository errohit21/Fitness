using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FLive.Infrastructure.Models;
using Serilog;
using System.Configuration;

namespace FLive.Infrastructure
{
    public class PaymentsWorker : BaseService
    {
        private readonly string _apiEndpoint;
        private readonly string _apiToken;
        private readonly string _dbConnection;
        private readonly StripeService _stripeService;

        public PaymentsWorker(string dbConnection, string apiEndpoint, string apiToken) : base(apiEndpoint, apiToken)
        {
            _dbConnection = dbConnection;
            _stripeService = new StripeService(_dbConnection);
        }

        public async Task TryChargeCompletedWorkouts()
        {
            var successful = true;
            var applicationFeePercentage =
                   Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationFeePercentage"]);

            using (var connection = new SqlConnection(_dbConnection))
            {

                connection.Open();

                var endTimeCutOff = DateTime.UtcNow.AddHours(-24*30).ToString("o");
                var query =
                    $"select lw.*,pt.Price,pt.Currency from LiveWorkouts lw join PriceTiers pt on lw.PriceTierId = pt.Id WHERE EndTime < '{endTimeCutOff}' and WorkoutStatus = 6 ";
                var liveWorkouts =
                    connection.Query<LiveWorkout>(query);
                //6=StreamDeleted

                Console.WriteLine(
                      $"Found  {liveWorkouts.Count()} workouts to be charged");
                foreach (var liveWorkout in liveWorkouts)
                {
                    Console.WriteLine($"Charging  the workout: {liveWorkout.Id}");
                    decimal totalForWorkout = 0;
                    var trainer = connection.Query<TrainerView>($"select * from  Trainers WHERE Id = {liveWorkout.TrainerId}").FirstOrDefault();

                    if(trainer == null)
                        continue;
                    connection.Execute($"update LiveWorkouts set WorkoutStatus=20  where Id={liveWorkout.Id}");
                    //20=charing strated
                  
                    var subscribers =
                        connection.Query<SubscriberView>(
                            $"select SubscriberId,StripeCustomerId,Currency from LiveWorkoutSubscriber lws join Subscribers s on lws.SubscriberId = s.Id where lws.LiveWorkoutId={liveWorkout.Id}");
                    Console.WriteLine($"Found  {subscribers.Count()} subscribers");

					var subscriberCount = subscribers.Count();
					switch (liveWorkout.PriceTierId)
					{
						case 1:
							if (subscriberCount >= 10)
								totalForWorkout = 1.94m + (subscriberCount - 10) * 0.3465m;
							Console.WriteLine($"Calculating for Price tier 1 - found {subscriberCount} subscribers. Earned USD {totalForWorkout} ");
							break;
						case 2:
							if (subscriberCount >= 5)
								totalForWorkout = 2.45m + (subscriberCount - 5) * 0.796m;
							Console.WriteLine($"Calculating for Price tier 2 - found {subscriberCount} subscribers. Earned USD {totalForWorkout} ");
							break;
						case 3:
							if (subscriberCount >= 2)
								totalForWorkout = 4.45m + (subscriberCount - 2) * 2.995m;
							Console.WriteLine($"Calculating for Price tier 2 - found {subscriberCount} subscribers. Earned USD {totalForWorkout} ");
							break;
					}
                   
					var currentTime = DateTime.UtcNow.ToString("o");
					connection.Execute(
						   $"insert into Transactions (TrainerId,LiveWorkoutId,SubscriberId,Amount,TranactionDateTime,IsSuccessfull,Notes) values ({liveWorkout.TrainerId},{liveWorkout.Id},{1},{liveWorkout.Price},'{currentTime}','{true}','{"completed"}')");

					var amountInString = totalForWorkout.ToString("F");
                    await SendPaymentReceivedNotification(GetHttpClient(), liveWorkout.Id, $"{liveWorkout.Currency} { amountInString}");
                    connection.Execute($"update LiveWorkouts set WorkoutStatus=21 where Id={liveWorkout.Id}");//7=FinishedCharingSubscribers
                }
            }
        }

        private int GetAmountInInt(decimal price)
        {
            return decimal.ToInt32(price*100);
        }
        private decimal GetAmountInDecimal(int price)
        {
            var val= (decimal)price / 100;
            return val;
        }

        public static string DoFormat(decimal myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }

        private static async Task SendPaymentReceivedNotification(HttpClient httpClient, long liveworkoutId,string amount)
        {
            try
            {
                var viewModel = BuildViewModel(liveworkoutId, amount);
                Console.WriteLine($"Sending notification : {viewModel}");

                var result =
                    await
                        httpClient.PostAsync("api/notifications/paymentreceived",
                            new StringContent(viewModel, Encoding.UTF8, "application/json"));
                var jsonResult = await result.Content.ReadAsStringAsync();
                
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to send notification {exception.Message}");
                Log.Logger.Error("Failed to send the payment received notification {exception}", exception);
            }
          
        }

        private static string BuildViewModel(long liveWorkoutId,string amount)
        {
            return $@"{{""LiveWorkoutId"":""{liveWorkoutId}"",""Amount"":""{amount}""}}";
        }


    }
}