using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Repositories;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stream = System.IO.Stream;

namespace FLive.Web.Services
{
	public class AzurePushNotifications : IAzurePushNotificationService
	{
		private const string ApiVersion = "api-version=2015-04";
		private const string ParticiapteInYourWorkout = "Has signed up in your workout";
		private const string WithdrawnFromYourWorkout = "Has withdrawn up in your workout";

		private const string MessageType_Participation = "participation";
		private const string MessageType_WorkoutGoingLive = "workout_going_live";
		private const string MessageType_Workout_Created = "workout_created";
		private const string MessageType_Workout_Published = "workout_published";
        private const string MessageType_PaymentReceived = "payment_received";
		private const string MessageType_LikedWorkout = "liked_workout";
		private const string MessageType_Followed = "followed";
		private const string MessageType_Registration = "registration";
		private const string MessageType_Cancellation = "cancellation";

		private const string YouHaveReceived = "You have received {0} in total for your last workout: {1}";
		private const string ForYourLastWorkout = "for your last workout: {0} ";
		private const string YouHaveScheduled = "You have scheduled a workout";
		private const string TrainerIsGoingLive = "{0} is going live with: {1} on {2}";
		private const string YouHaveSubscribed = "You have subscribed to the workout";
		private const string YouHaveWithdrawn = "You have withdrawn from the workout";
		private const string YourScheduledWorkoutIsLive = "Your scheduled workout is going live";
		private const string TrainerCancelledTheWorkout = "{0} cancelled : {1}";

		private const string LikedYourWorkout = "Liked your workout";
		private const string FollowedYou = "Followed you";
		private const string YouFollowed = "You followed";
		private const string YouLikedWorkout = "You liked workout";
		private const string PaymentReceived = "Payment received";
		private const string Cancelled = "Cancelled";

		private readonly ConnectionStringUtility _connectionStringUtility;
		private readonly string _defaultFullSharedAccessSignature;
		private HttpClient _httpClient;
		private readonly string _hubName;
		private readonly IGenericRepository<LiveWorkout> _liveWorkoutRepository;
		//private readonly ILiveWorkoutService _liveWorkoutService;
		private readonly IGenericRepository<LiveWorkoutSubscriber> _liveworkoutSubscribeRepository;
		private readonly IGenericRepository<Subscriber> _subscriberRepository;
		private readonly ILogger _logger;
		private readonly IGenericRepository<Notification> _notificationRepository;
		private readonly ServiceBusConfig _serviceBusConfig;
		private readonly ISubscriberService _subscriberService;
		private readonly ITrainerService _trainerService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IGenericRepository<SubscriberTrainingGoal> _subscriberTrainingGoalsRepository;

		private readonly string registrationXmlAndroid =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
			"<entry xmlns=\"http://www.w3.org/2005/Atom\">" +
			"<content type=\"application/xml\">" +
			"<GcmRegistrationDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" " +
			"xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\">" +
			"<Tags>general,othertags</Tags>" +
			"<GcmRegistrationId>{{DeviceToken}}" +
			"</GcmRegistrationId> " +
			"</GcmRegistrationDescription>" +
			"</content>" +
			"</entry>";

		private readonly string registrationXmlApple = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
													   "<entry xmlns=\"http://www.w3.org/2005/Atom\">" +
													   "<content type=\"application/xml\">" +
													   "<AppleRegistrationDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" " +
													   "xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\">" +
													   "<Tags>general,othertags</Tags>" +
													   "<DeviceToken>{{DeviceToken}}</DeviceToken> " +
													   "</AppleRegistrationDescription>" +
													   "</content>" +
													   "</entry>";

		public AzurePushNotifications(IOptions<ServiceBusConfig> serviceBusConfig, ILoggerFactory loggerFactory,
			UserManager<ApplicationUser> userManager,
			IGenericRepository<LiveWorkoutSubscriber> liveworkoutSubscribeRepository, ITrainerService trainerService,
			ISubscriberService subscriberService, IGenericRepository<Notification> notificationRepository,
			IGenericRepository<LiveWorkout> liveWorkoutRepository)
		{
			_httpClient = new HttpClient();
			_serviceBusConfig = serviceBusConfig.Value;
			_logger = loggerFactory.CreateLogger("notifications");
			_hubName = _serviceBusConfig.HubName;
			_defaultFullSharedAccessSignature = _serviceBusConfig.DefaultFullSharedAccessSignature;
			_connectionStringUtility = new ConnectionStringUtility(_defaultFullSharedAccessSignature);
			//_liveWorkoutService = liveWorkoutService;
			_userManager = userManager;
			_liveworkoutSubscribeRepository = liveworkoutSubscribeRepository;
			_trainerService = trainerService;
			_subscriberService = subscriberService;
			_liveWorkoutRepository = liveWorkoutRepository;
			_notificationRepository = notificationRepository;
		}

		public async Task Register(ApplicationUser applicationUser)
		{
			var tags = GetTags(applicationUser);
			await
				SendRegistrationRequest(string.Join(",", tags.ToArray()), applicationUser);

			var message = "Registration successful!";

			var notification = new Notification
			{
				Message = message,
				UserId = applicationUser.Id,
				MessageType = MessageType_Registration,
				PreviewImagelUrl = "",
				ProfileImage = ""
			};
			await
			  CreateNotificationInDb(notification);

			await SendNotification(applicationUser.Email, message, applicationUser);
		}

		public async Task UnRegister(ApplicationUser applicationUser)
		{
			//this has not been implemented as the push notifications rest api is too hard to work with. need to sort out when the Core package is avaialble for PushNotifications management. 
		}




		public async Task SendLiveWorkoutCreatedNotificaiton(LiveWorkoutNotificationModel liveWorkoutModel)
		{
			var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutModel.LiveWorkoutId);
			if (liveWorkout.IsNull())
			{
				_logger.LogError($"Workout with Id {liveWorkoutModel.LiveWorkoutId} not found");
				return;
			}
			var trainer = await _trainerService.GetWithSubscribers(liveWorkout.TrainerId);

			    var message =
				$"You have scheduled a workout: {liveWorkout.Title} on {liveWorkout.StartTime.ToString("D")} @ {liveWorkout.StartTime.ToString("T")}";
			//Todo convert time to user's time 

			var notification = new Notification
			{
				Title = YouHaveScheduled,
				Message = liveWorkout.Title,
				EventDateTime = DateTime.UtcNow,
				NotificationStatus = NotificationStatus.Created,
				UserId = trainer.ApplicationUser.Id,
				TrainerId = trainer.Id,
				ProfileImage = trainer.ApplicationUser.ProfileImageUrl,
				PreviewImagelUrl = liveWorkout.PreviewImagelUrl,
				UserName = "",
				MessageType = MessageType_Workout_Created
			};


			await
				CreateNotificationInDb(notification);

			await SendNotification(trainer.ApplicationUser.Email, message, trainer.ApplicationUser);


			//var subscriberMessage = string.Format(TrainerIsGoingLive, trainer.ApplicationUser.Name, liveWorkout.Title, liveWorkout.StartTime.ToString("D"));
			await NotifySubscribers(liveWorkout, trainer, false);

		}

        public async Task SendWorkoutPublishedNotification(LiveWorkoutNotificationModel liveWorkoutModel)
        {
            var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutModel.LiveWorkoutId);
            if (liveWorkout.IsNull())
            {
                _logger.LogError($"Workout with Id {liveWorkoutModel.LiveWorkoutId} not found");
                return;
            }
            var trainer = await _trainerService.GetWithSubscribers(liveWorkout.TrainerId);

            var message =
            $"Your workout: {liveWorkout.Title} is published now";
            //Todo convert time to user's time 

            var notification = new Notification
            {
                Title = YouHaveScheduled,
                Message = liveWorkout.Title,
                EventDateTime = DateTime.UtcNow,
                NotificationStatus = NotificationStatus.Created,
                UserId = trainer.ApplicationUser.Id,
                TrainerId = trainer.Id,
                ProfileImage = trainer.ApplicationUser.ProfileImageUrl,
                PreviewImagelUrl = liveWorkout.PreviewImagelUrl,
                UserName = "",
                MessageType = MessageType_Workout_Published
            };


            await
                CreateNotificationInDb(notification);

            await SendNotification(trainer.ApplicationUser.Email, message, trainer.ApplicationUser);

            await NotifySubscribers(liveWorkout, trainer, false);
        }

        public async Task SendLiveWorkoutCompletedNotificaiton(LiveWorkoutNotificationModel liveWorkoutModel)
		{
			var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutModel.LiveWorkoutId);
			if (liveWorkout.IsNull())
			{
				_logger.LogError($"Workout with Id {liveWorkoutModel.LiveWorkoutId} not found");
				return;
			}
			var trainer = await _trainerService.GetWithSubscribers(liveWorkout.TrainerId);

			await NotifySubscribers(liveWorkout, trainer, true);

		}

		private async Task NotifySubscribers(LiveWorkout liveWorkout, Trainer trainer, bool isCompleted)
		{
			foreach (var trainerSubscriber in trainer.TrainerFollowers)
            {

                string easternZoneId = "AUS Eastern Standard Time";
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                var ausTime = TimeZoneInfo.ConvertTime(liveWorkout.StartTime, easternZone);

                var subscriberMessage =
               isCompleted ? $"{trainer.ApplicationUser.Name}  just went live with: {liveWorkout.Title} " :
               $"{trainer.ApplicationUser.Name} is going live with: {liveWorkout.Title} on {liveWorkout.StartTime.ToString("D")} at {ausTime.ToString("f")} EST";


                //if(liveWorkout.WorkoutType == WorkoutType.Recorded)
                //{
                //    subscriberMessage = $"{trainer.ApplicationUser.Name}  just published a video : {liveWorkout.Title} ";
                //}
                if (liveWorkout.WorkoutType == 2)
                {
                    subscriberMessage = $"{trainer.ApplicationUser.Name}  just published a video : {liveWorkout.Title} ";
                }
                await SendSubscriberMessage(liveWorkout, trainer, trainerSubscriber, subscriberMessage);
            }
        }

        private async Task SendSubscriberMessage(LiveWorkout liveWorkout, Trainer trainer, TrainerFollower trainerSubscriber, string subscriberMessage )
        {
            var subscriberNotification = new Notification
            {
                Title = trainer.ApplicationUser.Name,
                Message = subscriberMessage,
                EventDateTime = DateTime.UtcNow,
                NotificationStatus = NotificationStatus.Created,
                UserId = trainerSubscriber.Subscriber.ApplicationUser.Id,
                TrainerId = trainer.Id,
                ProfileImage = trainer.ApplicationUser.ProfileImageUrl,
                PreviewImagelUrl = liveWorkout.PreviewImagelUrl,
                UserName = trainer.ApplicationUser.Name,
                //MessageType = liveWorkout.WorkoutType == WorkoutType.Live ?  MessageType_Workout_Created : MessageType_Workout_Published
                MessageType = liveWorkout.WorkoutType ==1 ? MessageType_Workout_Created : MessageType_Workout_Published
            };


            await
                CreateNotificationInDb(subscriberNotification);

            await SendNotification(trainerSubscriber.Subscriber.ApplicationUser.Email, subscriberMessage, trainerSubscriber.Subscriber.ApplicationUser);
        }

        public async Task SendFollowNotification(long trainerId, long userId)
		{
			var trainer = await _trainerService.Get(trainerId);
			var subscriber = await _subscriberService.Get(userId);
			var trainerMessage = $"User : {subscriber.ApplicationUser.Name} just followed you";
			var trainerNotification = new Notification
			{
				Title = FollowedYou,
				Message = "",
				NotificationStatus = NotificationStatus.Created,
				UserId = trainer.ApplicationUser.Id,
				ProfileImage = subscriber.ApplicationUser.ProfileImageUrl,
				PreviewImagelUrl = "",
				UserName = subscriber.ApplicationUser.Name,
				MessageType = MessageType_Followed
			};

			await
				CreateNotificationInDb(trainerNotification);

			await SendNotification(trainer.ApplicationUser.Email, trainerMessage, trainer.ApplicationUser);

			var subscriberNotification = new Notification
			{
				Title = YouFollowed,
				Message = "",
				UserId = subscriber.ApplicationUser.Id,
				ProfileImage = trainer.ApplicationUser.ProfileImageUrl,
				PreviewImagelUrl = "",
				UserName = trainer.ApplicationUser.Name,
				MessageType = MessageType_Followed,
				TrainerId = trainer.Id
			};

			await
				CreateNotificationInDb(subscriberNotification);
		}


		public async Task SendUserSubscribedNotificaiton(UserSubscribedNotificationModel notificationModel)
		{
			var liveWorkout = await _liveWorkoutRepository.GetById(notificationModel.LiveWorkoutId);

			if (liveWorkout.IsNull())
			{
				_logger.LogError($"Workout with Id {notificationModel.LiveWorkoutId} not found");
				return;
			}
			var trainer = await _trainerService.Get(liveWorkout.TrainerId);

			var trainerNotification = new Notification
			{
				Title = notificationModel.SubscriptionType == SubscriptionType.UnSubscribed ? WithdrawnFromYourWorkout : ParticiapteInYourWorkout,
				Message = notificationModel.WorkoutTitle,
				EventDateTime = DateTime.UtcNow,
				NotificationStatus = NotificationStatus.Created,
				UserId = notificationModel.TrainerUserId,
				ProfileImage = notificationModel.SubscriberProfileImage,
				PreviewImagelUrl = notificationModel.LiveWorkoutPreviewImageUrl,
				UserName = notificationModel.SubscriberName,
				MessageType = MessageType_Participation
			};

			var pushNotificationMessage = $"{trainerNotification.UserName} {trainerNotification.Title}";

			await
				CreateNotificationInDb(trainerNotification);

			//Notification for the User


			var subscriberNotificaiton = new Notification
			{
				Title = notificationModel.SubscriptionType == SubscriptionType.UnSubscribed ? YouHaveWithdrawn : YouHaveSubscribed,
				Message = notificationModel.WorkoutTitle,
				EventDateTime = DateTime.UtcNow,
				NotificationStatus = NotificationStatus.Created,
				UserId = notificationModel.SubscriberUserId,
				ProfileImage = trainer.ApplicationUser.ProfileImageUrl,
				PreviewImagelUrl = notificationModel.LiveWorkoutPreviewImageUrl,
				UserName = notificationModel.TrainerName,
				MessageType = MessageType_Participation,
				TrainerId = trainer.Id
			};


			await
				CreateNotificationInDb(subscriberNotificaiton);

			await SendNotification(trainer.ApplicationUser.Email, pushNotificationMessage, trainer.ApplicationUser);
		}

		public async Task<IEnumerable<Notification>> GetMyNotifications(string userId)
		{
			return
				await
					_notificationRepository.Vault.Where(a => a.UserId == userId)
						.OrderByDescending(a => a.EventDateTime)
						.ToListAsync();
		}

        public async Task<PaginatedResult<Notification>> GetMyNotificationsPaginated(string userId, int pageId) {

            return
                await
                    _notificationRepository
                    .GetMultiplePaginated(a => a.UserId == userId, 
                    a => a.EventDateTime, 
                    new string[] { },
                    pageId, 20);
            
        }

        public async Task SendUserLikedWorkoutNotificaiton(WorkoutLikedNotificationModel workoutLikedNotificationModel)
		{
			var trainerNotification = new Notification
			{
				Title = LikedYourWorkout,
				Message = workoutLikedNotificationModel.LiveWorkout.Title,
				EventDateTime = DateTime.UtcNow,
				NotificationStatus = NotificationStatus.Created,
				UserId = workoutLikedNotificationModel.Trainer.ApplicationUser.Id,
				ProfileImage = workoutLikedNotificationModel.Subscriber.ApplicationUser.ProfileImageUrl,
				PreviewImagelUrl = workoutLikedNotificationModel.LiveWorkout.PreviewImagelUrl,
				UserName = workoutLikedNotificationModel.Subscriber.ApplicationUser.Name,
				MessageType = MessageType_LikedWorkout
			};


			await
				CreateNotificationInDb(trainerNotification);

			var trainerMessage = $"{workoutLikedNotificationModel.Subscriber.ApplicationUser.Name} liked your workout :{workoutLikedNotificationModel.LiveWorkout.Title}";
			await SendNotification(workoutLikedNotificationModel.Trainer.ApplicationUser.Email, trainerMessage, workoutLikedNotificationModel.Trainer.ApplicationUser);

			var subscriberNotification = new Notification
			{
				Title = YouLikedWorkout,
				Message = workoutLikedNotificationModel.LiveWorkout.Title,
				EventDateTime = DateTime.UtcNow,
				NotificationStatus = NotificationStatus.Created,
				UserId = workoutLikedNotificationModel.Subscriber.ApplicationUser.Id,
				ProfileImage = workoutLikedNotificationModel.Trainer.ApplicationUser.ProfileImageUrl,
				PreviewImagelUrl = workoutLikedNotificationModel.LiveWorkout.PreviewImagelUrl,
				UserName = workoutLikedNotificationModel.Trainer.ApplicationUser.Name,
				MessageType = MessageType_LikedWorkout,
				TrainerId = workoutLikedNotificationModel.Trainer.Id
			};


			await
				CreateNotificationInDb(subscriberNotification);
			await SendNotification(workoutLikedNotificationModel.Trainer.ApplicationUser.Email, trainerMessage, workoutLikedNotificationModel.Trainer.ApplicationUser);


			//push notifications


		}

		public async Task SendWorkoutCancelledNotification(LiveWorkoutSubscriber subscriber, LiveWorkout workout)
		{
			var message = string.Format(TrainerCancelledTheWorkout, workout.Trainer.ApplicationUser.Name, workout.Title);

			var subscriberUser = await _subscriberService.Get(subscriber.SubscriberId);
			var notificaiton = new Notification
			{
				Title = Cancelled,
				Message = workout.Title,
				UserId = subscriberUser.ApplicationUser.Id,
				ProfileImage = workout.Trainer.ApplicationUser.ProfileImageUrl,
				PreviewImagelUrl = workout.PreviewImagelUrl,
				UserName = workout.Trainer.ApplicationUser.Name,
				MessageType = MessageType_Cancellation
			};

			await
				CreateNotificationInDb(notificaiton);
			await SendNotification(subscriberUser.ApplicationUser.Email, message, subscriberUser.ApplicationUser);

		}


		public async Task SendWorkoutCancelledNotification(Trainer trainer, LiveWorkout workout)
		{

			var notificaiton = new Notification
			{
				Title = $"You cancelled the workout {workout.Title}",
				Message = "",
				UserId = trainer.ApplicationUser.Id,
				ProfileImage = "",
				PreviewImagelUrl = workout.PreviewImagelUrl,
				UserName = "",
				MessageType = MessageType_Cancellation
			};

			await
				CreateNotificationInDb(notificaiton);

		}
		public async Task PaymentReceivedNotificaiton(PaymentReceivedNotificationModel paymentReceivedNotificationModel)
		{

			var liveWorkout = await _liveWorkoutRepository.Vault.Include(a => a.Trainer.ApplicationUser)
				.FirstOrDefaultAsync(a => a.Id == paymentReceivedNotificationModel.LiveWorkoutId);

			var message = string.Format(YouHaveReceived, paymentReceivedNotificationModel.Amount.ToString(), liveWorkout.Title);

			var notificaiton = new Notification
			{
				Title = PaymentReceived,
				Message = message,
				EventDateTime = DateTime.UtcNow,
				NotificationStatus = NotificationStatus.Created,
				UserId = liveWorkout.Trainer.ApplicationUser.Id,
				ProfileImage = "",
				PreviewImagelUrl = "",
				UserName = "",
				MessageType = MessageType_PaymentReceived
			};

			await
				CreateNotificationInDb(notificaiton);

			await SendNotification(liveWorkout.Trainer.ApplicationUser.Email, message, liveWorkout.Trainer.ApplicationUser);

		}

		public async Task SendLiveWorkoutReminder(LiveWorkoutNotificationModel liveWorkoutModel)
		{
			var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutModel.LiveWorkoutId);

			if (liveWorkout.IsNull())
			{
				_logger.LogError($"Workout with Id {liveWorkoutModel.LiveWorkoutId} not found");
				return;
			}
			var trainer = await _trainerService.Get(liveWorkout.TrainerId);

			var timeRemaining = liveWorkout.StartTime.Subtract(DateTime.UtcNow);

			var trainerMessage = $"Your live workout is starting in {timeRemaining.Minutes} minutes";
			await SendNotification(trainer.ApplicationUser.Email, trainerMessage, trainer.ApplicationUser);

			await PersistTrainerNotification(liveWorkout, trainer, trainerMessage);

			var subscriberMessage =
				$"Your live workout with {trainer.ApplicationUser.Name} is starting in {timeRemaining.Minutes} minutes";
			var subscribers =
				await
					_liveworkoutSubscribeRepository.Vault.Include(a => a.Subscriber.ApplicationUser)
						.Where(a => a.LiveWorkoutId == liveWorkout.Id)
						.ToListAsync();

			foreach (var subscriber in subscribers)
			{
				await
					SendNotification(subscriber.Subscriber.ApplicationUser.Email, subscriberMessage,
						subscriber.Subscriber.ApplicationUser);

				await PersistSubscriberNotification(liveWorkout, subscriber, trainer, subscriberMessage);
			}
		}

		private async Task PersistSubscriberNotification(LiveWorkout liveWorkout, LiveWorkoutSubscriber subscriber,
			Trainer trainer, string subsriberMessage)
		{
			var subscriberNotificaiton = new Notification
			{
				Title = YourScheduledWorkoutIsLive,
				Message = subsriberMessage,
				UserId = subscriber.Subscriber.ApplicationUser.Id,
				PreviewImagelUrl = liveWorkout.PreviewImagelUrl,
				MessageType = MessageType_WorkoutGoingLive,
				ProfileImage = trainer.ApplicationUser.ProfileImageUrl,
				UserName = trainer.ApplicationUser.Name,
				TrainerId = trainer.Id,
				WorkoutEndTime = liveWorkout.EndTime,
				WorkoutStartTime = liveWorkout.StartTime
			};
			await CreateNotificationInDb(subscriberNotificaiton);
		}

		private async Task PersistTrainerNotification(LiveWorkout liveWorkout, Trainer trainer, string message)
		{
			var trainerNotification = new Notification
			{
				Title = YourScheduledWorkoutIsLive,
				Message = message,
				PreviewImagelUrl = liveWorkout.PreviewImagelUrl,
				UserName = "",
				WorkoutEndTime = liveWorkout.EndTime,
				WorkoutStartTime = liveWorkout.StartTime,
				MessageType = MessageType_WorkoutGoingLive,
				ProfileImage = "",
				UserId = trainer.ApplicationUser.Id
			};


			await CreateNotificationInDb(trainerNotification);
		}

		private async Task CreateNotificationInDb(Notification notification)
		{
			await
				_notificationRepository.Add(notification);
		}

		public IList<string> GetTags(ApplicationUser applicationUser)
		{
			var tags = new List<string>();
			var uniqueTag = applicationUser.Email;
			tags.Add(uniqueTag);

			switch (applicationUser.UserType)
			{
				case UserType.Subscriber:
					tags.Add("allsubscribers");
					break;
				case UserType.Trainer:
					tags.Add("alltrainers");
					break;
				default:
					tags.Add("alltrainers");
					tags.Add("allsubscribers");
					break;
			}
			return tags;
		}

		public string GetPlatformName(ApplicationUser applicationUser)
		{
			switch (applicationUser.Platform)
			{
				case "Apple":
					return "APNS";
				case "Android":
					return "GCM";
				default:
					return "WNS";
			}
		}

		private async Task SendNotification(string tags, string message,
			ApplicationUser applicationUser)
		{
			string location = null;
			var hubResource = "messages/?";
			var uri = _connectionStringUtility.Endpoint + _hubName + "/" + hubResource + ApiVersion;
			var SasToken = _connectionStringUtility.getSaSToken(uri, 10);

			_httpClient = new HttpClient();
			string body;
			var nativeType = GetPlatformName(applicationUser);

			switch (nativeType.ToLower())
			{
				case "apns":

					_httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Format", "apple");
					_httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Tags", tags);

					body = "{\"aps\":{\"alert\":\"" + message + "\"}}";
					await ExecuteREST("POST", uri, SasToken, body);
					break;

				case "gcm":
					_httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Format", "gcm");
					body = "{\"data\":{\"message\":\"" + message + "\"}}";
					await ExecuteREST("POST", uri, SasToken, body);
					break;
			}
		}

		private async Task SendRegistrationRequest(string tags,
			ApplicationUser applicationUser)
		{
			var hubResource = "registrations/?";
			var apiVersion = "api-version=2015-04";
			var uri = _connectionStringUtility.Endpoint + _hubName + "/" + hubResource + apiVersion;
			var SasToken = _connectionStringUtility.getSaSToken(uri, 10);

			string body;

			var nativeType = GetPlatformName(applicationUser);
			var deviceToken = applicationUser.DeviceToken;
			switch (nativeType.ToLower())
			{
				case "apns":

					// _httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Format", "apple");
					body = registrationXmlApple
						.Replace("othertags", tags)
						.Replace("{{DeviceToken}}", deviceToken);

					await ExecuteREST("POST", uri, SasToken, body, "application/xml");
					break;

				case "gcm":
					//_httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Format", "gcm");
					body = registrationXmlAndroid
						.Replace("othertags", tags)
						.Replace("{{DeviceToken}}", deviceToken); ;
					await ExecuteREST("POST", uri, SasToken, body, "application/xml");
					break;
			}
		}

		private async Task<HttpResponseMessage> ExecuteREST(string httpMethod, string uri, string sasToken,
			string body = null, string contentType = "application/json")
		{
			if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
				_httpClient.DefaultRequestHeaders.Remove("Authorization");

			if (sasToken != null)
				_httpClient.DefaultRequestHeaders.Add("Authorization", sasToken);

			_httpClient.DefaultRequestHeaders
				.Accept
				.Add(new MediaTypeWithQualityHeaderValue(contentType)); //ACCEPT header

			try
			{


				HttpResponseMessage response = null;
				if (httpMethod == "POST")
				{
					response = await _httpClient.PostAsync(uri, new StringContent(body, Encoding.UTF8,
						contentType));
				}
				else if (httpMethod == "GET")
				{
					response = await _httpClient.GetAsync(uri);
				}
				else if (httpMethod == "DELETE")
				{
					response = await _httpClient.DeleteAsync(uri);
				}

				if (!(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK))
				{
					_logger.LogError($"Error {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}
			return new HttpResponseMessage();
		}

		private async Task DisplayResponseBody(HttpResponseMessage response, string forcedType = null)
		{
			if (response == null)
				return;

			var contentType = response.Content.Headers.ContentType.MediaType;
			if (forcedType != null)
				contentType = forcedType;

			// Get the stream associated with the response.
			var receiveStream = await response.Content.ReadAsStreamAsync();

			// Pipes the stream to a higher level stream reader with the required encoding format. 
			var readStream = new StreamReader(receiveStream, Encoding.UTF8);

			Console.WriteLine("");

			if (receiveStream == null)
				return;


			if (contentType.Contains("application/octet-stream"))
			{
				var xmlFiles = readStream.ReadToEnd();
				string[] sseps = { "<?xml " };
				var docs = xmlFiles.Split(sseps, StringSplitOptions.RemoveEmptyEntries);

				StringBuilder sb = null;
				XmlDocument xml = null;
				var settings = new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "  ",
					NewLineChars = "\r\n",
					NewLineHandling = NewLineHandling.Replace
				};

				foreach (var doc in docs)
				{
					xml = new XmlDocument();
					xml.LoadXml(sseps[0] + doc);
					sb = new StringBuilder();

					using (var writer = XmlWriter.Create(sb, settings))
					{
						xml.Save(writer);
					}

					Console.WriteLine(sb + "\n");
				}
			}

			if (contentType.Contains("application/xml"))
			{
				var xml = new XmlDocument();
				xml.LoadXml(readStream.ReadToEnd());

				var sb = new StringBuilder();
				var settings = new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "  ",
					NewLineChars = "\r\n",
					NewLineHandling = NewLineHandling.Replace
				};

				using (var writer = XmlWriter.Create(sb, settings))
				{
					xml.Save(writer);
				}

				Console.WriteLine(sb.ToString());
			}

			if (contentType.Contains("application/json"))
			{
				Console.WriteLine(JsonHelper.FormatJson(readStream.ReadToEnd()));
			}

			//readStream.Close();
			//receiveStream.Close();
		}

		private class JsonHelper
		{
			private const string INDENT_STRING = "  ";

			public static string FormatJson(string str)
			{
				var indent = 0;
				var quoted = false;
				var sb = new StringBuilder();
				for (var i = 0; i < str.Length; i++)
				{
					var ch = str[i];
					switch (ch)
					{
						case '{':
						case '[':
							sb.Append(ch);
							if (!quoted)
							{
								sb.AppendLine();
								Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
							}
							break;
						case '}':
						case ']':
							if (!quoted)
							{
								sb.AppendLine();
								Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
							}
							sb.Append(ch);
							break;
						case '"':
							sb.Append(ch);
							var escaped = false;
							var index = i;
							while (index > 0 && str[--index] == '\\')
								escaped = !escaped;
							if (!escaped)
								quoted = !quoted;
							break;
						case ',':
							sb.Append(ch);
							if (!quoted)
							{
								sb.AppendLine();
								Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
							}
							break;
						case ':':
							sb.Append(ch);
							if (!quoted)
								sb.Append(" ");
							break;
						default:
							sb.Append(ch);
							break;
					}
				}
				return sb.ToString();
			}
		}

		#region unused methods 

		public async Task Delete()
		{
			await DeleteAllRegistraions();
		}

		private async Task<HttpResponseMessage> GetNotificationTelemtry(string id, string hubname,
			string connectionString)
		{
			var hubResource = "messages/" + id + "?";
			var apiVersion = "api-version=2015-04";
			var connectionSasUtil = new ConnectionStringUtility(connectionString);

			//=== Generate SaS Security Token for Authentication header ===
			// Determine the targetUri that we will sign
			var uri = connectionSasUtil.Endpoint + hubname + "/" + hubResource + apiVersion;
			var SasToken = connectionSasUtil.getSaSToken(uri, 60);

			return await ExecuteREST("GET", uri, SasToken);
		}


		private async Task<string> GetPlatformNotificationServiceFeedbackContainer(string hubName,
			string connectionString)
		{
			HttpResponseMessage response = null;
			var connectionSasUtil = new ConnectionStringUtility(connectionString);

			var hubResource = "feedbackcontainer?";
			var apiVersion = "api-version=2015-04";

			//=== Generate SaS Security Token for Authentication header ===
			// Determine the targetUri that we will sign
			var uri = connectionSasUtil.Endpoint + hubName + "/" + hubResource + apiVersion;

			// 10 min expiration
			var SasToken = connectionSasUtil.getSaSToken(uri, 10);
			response = await ExecuteREST("GET", uri, SasToken);

			if ((int)response.StatusCode != 200)
			{
				Console.WriteLine(string.Format("Failed to get PNS feedback contaioner URI - Http Status {0} : {1}",
					(int)response.StatusCode, response.StatusCode));

				// Get the stream associated with the response.
				var errorStream = await response.Content.ReadAsStreamAsync();

				// Pipes the stream to a higher level stream reader with the required encoding format. 
				var errorReader = new StreamReader(errorStream, Encoding.UTF8);
				Console.WriteLine("\n" + errorReader.ReadToEnd());

				return null;
			}

			// Get the stream associated with the response.
			var receiveStream = await response.Content.ReadAsStreamAsync();

			// Pipes the stream to a higher level stream reader with the required encoding format. 
			var readStream = new StreamReader(receiveStream, Encoding.UTF8);
			Console.WriteLine("");
			var containerUri = readStream.ReadToEnd();


			return containerUri;
		}

		private async Task DeleteAllRegistraions()
		{
			var hubName = _serviceBusConfig.HubName;
			var fullConnectionString = _serviceBusConfig.DefaultFullSharedAccessSignature;

			var connectionSaSUtil = new ConnectionStringUtility(fullConnectionString);
			string location = null;

			var hubResource = "registrations/";
			var apiVersion = "api-version=2015-04";
			var notificationId = "Failed to get Notification Id";

			//=== Generate SaS Security Token for Authentication header ===
			// Determine the targetUri that we will sign
			var uri = connectionSaSUtil.Endpoint + hubName + "/" + hubResource + apiVersion;


			_httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Format", "apple");
			_httpClient.DefaultRequestHeaders.Add("x-ms-version", "2015-01");
			_httpClient.DefaultRequestHeaders.Add("If-Match", "*");


			// 10 min expiration


			var registrations = new[]
			{
				"8427120814944195789-2942513711250288575-1"
			};
			foreach (var reg in registrations)
			{
				var uri1 = connectionSaSUtil.Endpoint + hubName + "/" + hubResource + $"{reg}?" + apiVersion;
				var SasToken = connectionSaSUtil.getSaSToken(uri1, 10);

				//var uri2 = connectionSaSUtil.Endpoint + hubName + "/" + hubResource+ "1375044744274851654-5433086163058464900-1?" + apiVersion;
				//var uri3 = connectionSaSUtil.Endpoint + hubName + "/" + hubResource+ "84873369198524259189-1861850336084594272-1?" + apiVersion;

				var response = await ExecuteREST("DELETE", uri1, SasToken);
			}

			//response = await ExecuteREST("DELETE", uri2, SasToken);
			//response = await ExecuteREST("DELETE", uri3, SasToken);
		}


		private async Task WalkBlobContainer(string containerUri)
		{
			var listcontainerUri = containerUri + "&restype=container&comp=list";

			var response = await ExecuteREST("GET", listcontainerUri, null);

			// Get Blob name
			Stream receiveStreamContainer = null;
			StreamReader readStreamContainer = null;

			if (((int)response.StatusCode == 200) && response.Content.Headers.Contains("application/xml"))
			{
				// Get the stream associated with the response.
				receiveStreamContainer = await response.Content.ReadAsStreamAsync();

				// Pipes the stream to a higher level stream reader with the required encoding format. 
				readStreamContainer = new StreamReader(receiveStreamContainer, Encoding.UTF8);

				if (readStreamContainer != null)
				{
					var xml = new XmlDocument();
					xml.LoadXml(readStreamContainer.ReadToEnd());


					var sb = new StringBuilder();
					var settings = new XmlWriterSettings
					{
						Indent = true,
						IndentChars = "  ",
						NewLineChars = "\r\n",
						NewLineHandling = NewLineHandling.Replace
					};

					using (var writer = XmlWriter.Create(sb, settings))
					{
						xml.Save(writer);
					}

					Console.WriteLine(sb + "\n\n");


					var list = xml.GetElementsByTagName("Blob");

					string[] parts = null;
					char[] seps = { '?' };
					string blobURL = null;

					foreach (XmlNode node in list)
					{
						Console.WriteLine("Get Blob named : " + node["Name"].InnerText);
						parts = containerUri.Split(seps);
						blobURL = parts[0] + "/" + node["Name"].InnerText + "?" + parts[1];
						Console.WriteLine("Blob URL : " + blobURL);
						response = await ExecuteREST("GET", blobURL, null);
						DisplayResponseBody(response);
					}
				}
			}
		}

		#endregion
	}

	internal static class Extensions
	{
		public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
		{
			foreach (var i in ie)
			{
				action(i);
			}
		}
	}
}