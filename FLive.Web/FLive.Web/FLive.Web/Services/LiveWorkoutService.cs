using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Repositories;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FLive.Web.Services
{
	public class LiveWorkoutService : ILiveWorkoutService
	{
		private readonly IGenericRepository<LiveWorkoutFavor> _liveworkoutFavorRepository;
		private readonly IGenericRepository<LiveWorkout> _liveWorkoutRepository;
		private readonly IGenericRepository<LiveWorkoutSubscriber> _liveworkoutSubscriberRepository;
		private readonly IGenericRepository<LiveWorkoutSubscriber> _liveWorkoutSubsriberRepository;
		private readonly IAzurePushNotificationService _notificationsService;
        private readonly IPaymentGatewayServie _paymentGatewayServie;
		private readonly IStreamManagementService _streamManagementService;
		private readonly ISubscriberService _subscriberService;
		private readonly ITrainerService _trainerService;
		private readonly IGenericRepository<TrainingCategory> _trainingCategoryRepository;
		private readonly IGenericRepository<UpcomingLiveWorkout> _upcomingLiveWorkoutRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public LiveWorkoutService(IGenericRepository<LiveWorkout> liveWorkoutRepository,
			IGenericRepository<LiveWorkoutSubscriber> liveWorkoutSubsriberRepository,
			IStreamManagementService streamManagementService, IPaymentGatewayServie paymentGatewayServie,
			IGenericRepository<UpcomingLiveWorkout> upcomingLiveWorkoutRepository,
			UserManager<ApplicationUser> userManager,
			IGenericRepository<LiveWorkoutSubscriber> liveworkoutSubscriberRepository,
			ISubscriberService subscriberService,
			ITrainerService trainerService,
			IGenericRepository<TrainingCategory> trainingCategoryRepository,
			IGenericRepository<LiveWorkoutFavor> liveworkoutFavorRepository,
            IAzurePushNotificationService notificationsService)
		{
			_liveWorkoutRepository = liveWorkoutRepository;
			_liveWorkoutSubsriberRepository = liveWorkoutSubsriberRepository;
			_streamManagementService = streamManagementService;
			_paymentGatewayServie = paymentGatewayServie;
			_upcomingLiveWorkoutRepository = upcomingLiveWorkoutRepository;
			_userManager = userManager;
			_liveworkoutSubscriberRepository = liveworkoutSubscriberRepository;
			_subscriberService = subscriberService;
			_trainerService = trainerService;
			_trainingCategoryRepository = trainingCategoryRepository;
			_liveworkoutFavorRepository = liveworkoutFavorRepository;
			_notificationsService = notificationsService;
        }

		public async Task<WorkoutCreateResult> Create(string currentUserId, LiveEventViewModel liveEvent)
		{
			var trainer = await _trainerService.GetByUserId(currentUserId);

			var timeZone = trainer.ApplicationUser.Timezone ?? "Australia/Melbourne";

			var trainingCategory = await ValidateAndGetCategory(liveEvent.TrainingCategoryId);


            var eventStartTimeInUtc = DateTime.Now;
            var endTime = DateTime.Now;
            //if (liveEvent.WorkoutType == WorkoutType.Live)
            //{
            //     endTime = eventStartTimeInUtc.AddMinutes(liveEvent.DurationMins);
            //     eventStartTimeInUtc = liveEvent.StartTime.ToUTCTime(timeZone);

            //    await ValidateTime(eventStartTimeInUtc, endTime, trainer.Id);
            //}
            if (liveEvent.WorkoutType == 1)
            {
                endTime = eventStartTimeInUtc.AddMinutes(liveEvent.DurationMins);
                eventStartTimeInUtc = liveEvent.StartTime.ToUTCTime(timeZone);

                await ValidateTime(eventStartTimeInUtc, endTime, trainer.Id);
            }
            else
            {
                if (liveEvent.IsPublishImmediately)
                {
                    eventStartTimeInUtc = DateTime.UtcNow.AddHours(1);//just a ballpark figure to give enough time to encoding to finish
                }
                else
                {
                    eventStartTimeInUtc = liveEvent.PublishDateTime.ToUTCTime(timeZone);
                }
                endTime = eventStartTimeInUtc.AddHours(1); // get the duration from media services? 


            }

            var externalAccessToken = Guid.NewGuid().ToString();

            var liveWorkout = new LiveWorkout
            {
                Title = liveEvent.Title,
                CreatedTime = DateTime.UtcNow,
                DurationMins = liveEvent.DurationMins,
                PriceTierId = liveEvent.PriceTierId,
                StartTime = eventStartTimeInUtc,
                EndTime = endTime,
                TrainingCategoryId = liveEvent.TrainingCategoryId,
                WorkoutLevel = liveEvent.WorkoutLevel,
                TrainerId = trainer.Id,
                //WorkoutType = liveEvent.WorkoutType,
                WorkoutType = liveEvent.WorkoutType,
                PublishDateTime = eventStartTimeInUtc,
                ExternalAccessToken = externalAccessToken,
                //IsArchived = liveEvent.WorkoutType == WorkoutType.Recorded 
                IsArchived = liveEvent.WorkoutType  == 2
            };
			var newWorkoutId = await _liveWorkoutRepository.Add(liveWorkout);


			var wowzaRegion = GetWowzaRegion(timeZone);
            await _upcomingLiveWorkoutRepository.Add(new UpcomingLiveWorkout
            {
                LiveWorkoutId = newWorkoutId,
                StartTime = eventStartTimeInUtc,
                CreateStreamJson = string.Format(@"{{""WowzaRegion"":""{0}""}}", wowzaRegion),
                WorkoutType = liveEvent.WorkoutType,
                IsPublishImmediately = liveEvent.IsPublishImmediately,
                PublishDateTime = liveEvent.PublishDateTime

            });

            //if (liveEvent.WorkoutType == WorkoutType.Live)
            //{
            //    await _notificationsService.SendLiveWorkoutCreatedNotificaiton(new LiveWorkoutNotificationModel { LiveWorkoutId = newWorkoutId });
            //}
            if (liveEvent.WorkoutType == 1)
            {
                await _notificationsService.SendLiveWorkoutCreatedNotificaiton(new LiveWorkoutNotificationModel { LiveWorkoutId = newWorkoutId });
            }
            await UpdateLiveworkoutCount(trainingCategory);
			return  new WorkoutCreateResult { WorkoutId = newWorkoutId, ExternalAccessToken = externalAccessToken };
		}



        public async Task Delete(long workoutId, string userId)
		{
			var trainer = await _trainerService.GetByUserId(userId);
			var workout = await Get(workoutId);

			if (trainer.Id != workout.TrainerId)
				throw new BusinessException("Only the owner can delete the workout");

            //if (workout.WorkoutType == WorkoutType.Live && workout.StartTime < DateTime.UtcNow.AddMinutes(30))
            //	throw new BusinessException("Cannot delete a workout 30 mins prior to the start");
            if (workout.WorkoutType == 1 && workout.StartTime < DateTime.UtcNow.AddMinutes(30))
                throw new BusinessException("Cannot delete a workout 30 mins prior to the start");

            var subscribers = await _liveworkoutSubscriberRepository.Vault.Where(a => a.LiveWorkoutId == workoutId).ToListAsync();
			foreach (var subscriber in subscribers)
			{
				await _notificationsService.SendWorkoutCancelledNotification(subscriber, workout);
				await _liveworkoutSubscriberRepository.Remove(subscriber);
			}

			await _notificationsService.SendWorkoutCancelledNotification(trainer, workout);
			await _liveWorkoutRepository.Remove(workout);
		}

		public async Task SetPreviewVideo(long liveWorkoutId, string uploadedUrl)
		{
			var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutId);

			if (liveWorkout.IsNull())
			{
				throw new BusinessException("Liveworkout could not be found!");
			}

			liveWorkout.PreviewVideoUrl = uploadedUrl;
			await _liveWorkoutRepository.Save();
		}

        public async Task SetInstructionsFile(long liveWorkoutId, string uploadedUrl)
        {
            var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutId);

            if (liveWorkout.IsNull())
            {
                throw new BusinessException("Liveworkout could not be found!");
            }

            liveWorkout.InstructionsLink = uploadedUrl;
            await _liveWorkoutRepository.Save();
        }



        public async Task SetPreviewImage(long liveWorkoutId, string uploadedUrl)
		{
			var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutId);

			if (liveWorkout.IsNull())
			{
				throw new BusinessException("Liveworkout could not be found!");
			}

			liveWorkout.PreviewImagelUrl = uploadedUrl;
			await _liveWorkoutRepository.Save();
		}

		public async Task<LiveWorkout> Get(long workoutId, long? subscriberId = null)
		{
			var workout =
				await _liveWorkoutRepository.Vault
					.Include(a => a.LiveWorkoutSubscribers)
					.Include(a => a.Trainer.ApplicationUser)
					.Include(a => a.Trainer.TrainerSubscribers)
					.Include(a => a.LiveWorkoutFavors)
					.Include(a => a.PriceTier)
					.Where(a => a.Id == workoutId)
					.FirstOrDefaultAsync();

			HydrateNotMappedProperties(subscriberId, workout);
			return workout;
		}

		public async Task<IEnumerable<LiveWorkout>> GetCurrentLiveWorkouts(long? subscriberId = null)
		{
			var currentTime = DateTime.UtcNow;
			var candidates = await _liveWorkoutRepository.Vault.Include(a => a.LiveWorkoutSubscribers)
				.Include(a => a.Trainer.ApplicationUser)
				.Include(a => a.Trainer.TrainerSubscribers)
				 .Include(a => a.PriceTier)
				.Where(a => a.StartTime < currentTime && a.EndTime > currentTime)
				.ToListAsync();


			foreach (var candidate in candidates)
			{
				HydrateNotMappedProperties(subscriberId, candidate);
			}

			return candidates;
		}

		public async Task<IEnumerable<LiveWorkout>> GetUpcomingVideosByUser(string userId)
		{
			var currentTime = DateTime.UtcNow;
			var subscriber = await _subscriberService.GetByUserId(userId);
			if (subscriber.IsNull())
				throw new BusinessException($"Subscriber with user id {userId} not found");

			var upcomingWorkoutsByUser = await
				_liveWorkoutSubsriberRepository.Vault
					.Include(a => a.LiveWorkout.Trainer.ApplicationUser)
					.Include(a => a.LiveWorkout.LiveWorkoutSubscribers)
					.Include(a => a.LiveWorkout.PriceTier)
					.Where(
						a =>
							a.LiveWorkout.EndTime > currentTime &&
							a.SubscriberId == subscriber.Id)
					.ToListAsync();

			var candidates = upcomingWorkoutsByUser.Select(a => a.LiveWorkout).OrderBy(a => a.StartTime);
			foreach (var candidate in candidates)
			{
				HydrateNotMappedProperties(subscriber.Id, candidate);
			}

			return candidates;
		}

		public async Task Subscribe(long liveWorkoutId, long subscriberId)
		{
			var existingSubscription =
				await _liveWorkoutSubsriberRepository.GetFirstOrDefault(
					a => a.SubscriberId == subscriberId && a.LiveWorkoutId == liveWorkoutId);
			if (existingSubscription.IsNotNull())
				throw new BusinessException(
					$"A subscrioption already exists for user {subscriberId} for workout {liveWorkoutId}");


			var liveWorkout =
				await
					_liveWorkoutRepository.Vault.Include(a => a.Trainer.ApplicationUser)
						.FirstOrDefaultAsync(a => a.Id == liveWorkoutId);
			liveWorkout.SubscriberCount = liveWorkout.SubscriberCount + 1;

			await _liveWorkoutRepository.Save();
			var subscriber = await _subscriberService.Get(subscriberId);
			await
				_notificationsService.SendUserSubscribedNotificaiton(new UserSubscribedNotificationModel
				{
					SubscriberId = subscriberId,
					LiveWorkoutId = liveWorkoutId,
					WorkoutTitle = liveWorkout.Title,
					SubscriberName = subscriber.ApplicationUser.Name,
					SubscriberUserId = subscriber.ApplicationUser.Id,
					TrainerUserId = liveWorkout.Trainer.ApplicationUser.Id,
					TrainerName = liveWorkout.Trainer.ApplicationUser.Name,
					SubscriberProfileImage = subscriber.ApplicationUser.ProfileImageUrl,
					LiveWorkoutPreviewImageUrl = liveWorkout.PreviewImagelUrl

				});

			await _liveWorkoutSubsriberRepository.Add(new LiveWorkoutSubscriber
			{
				LiveWorkoutId = liveWorkoutId,
				SubscriberId = subscriberId
			});



		}

		public async Task UnSubscribe(long liveWorkoutId, long subscriberId)
		{
			var existingSubscription =
				await _liveWorkoutSubsriberRepository.GetFirstOrDefault(
					a => a.SubscriberId == subscriberId && a.LiveWorkoutId == liveWorkoutId);
			if (existingSubscription.IsNull())
				throw new BusinessException(
					$"No subscription found for user {subscriberId} for workout {liveWorkoutId}");


			var liveWorkout =
				await
					_liveWorkoutRepository.Vault.Include(a => a.Trainer.ApplicationUser)
						.FirstOrDefaultAsync(a => a.Id == liveWorkoutId);
			liveWorkout.SubscriberCount = liveWorkout.SubscriberCount - 1;

			await _liveWorkoutRepository.Save();
			var subscriber = await _subscriberService.Get(subscriberId);
			await
				_notificationsService.SendUserSubscribedNotificaiton(new UserSubscribedNotificationModel
				{
					SubscriberId = subscriberId,
					LiveWorkoutId = liveWorkoutId,
					WorkoutTitle = liveWorkout.Title,
					SubscriberName = subscriber.ApplicationUser.Name,
					SubscriberUserId = subscriber.ApplicationUser.Id,
					TrainerUserId = liveWorkout.Trainer.ApplicationUser.Id,
					TrainerName = liveWorkout.Trainer.ApplicationUser.Name,
					SubscriberProfileImage = subscriber.ApplicationUser.ProfileImageUrl,
					LiveWorkoutPreviewImageUrl = liveWorkout.PreviewImagelUrl,
					SubscriptionType = SubscriptionType.UnSubscribed

				});

			await _liveWorkoutSubsriberRepository.Remove(existingSubscription);



		}

		public async Task<IEnumerable<Subscriber>> GetSubscribers(long liveWorkoutId)
		{
			return await _liveWorkoutSubsriberRepository.Vault.Where(a => a.LiveWorkoutId == liveWorkoutId)
				.Include(a => a.Subscriber)
				.Select(a => a.Subscriber)
				.ToArrayAsync();
		}

		public async Task CompleteWorkout(long liveWorkoutId)
		{
			//var allSubscribers =
			//    await _liveWorkoutSubsriberRepository.GetMultiple(a => a.LiveWorkoutId == liveWorkoutId);
			//foreach (var subscriber in allSubscribers)
			//{
			//    await _paymentGatewayServie.Charge(subscriber.Id);
			//}
			await _notificationsService.SendLiveWorkoutCompletedNotificaiton(new LiveWorkoutNotificationModel { LiveWorkoutId = liveWorkoutId });
		}

		public async Task<UpcomingWorkoutsContainer> GetUpcomingWorkouts(long? subscriberId = null)
		{
			var allCandidates = await GetFirst20UpcomingLiveWorkouts(subscriberId);
			var groupByCategory = allCandidates.GroupBy(a => a.TrainingCategoryId);

			var upcomingWorkoutContainer = new UpcomingWorkoutsContainer();
			foreach (var categoryGroup in groupByCategory)
			{
				var workoutCategoryContainer = new WorkoutCategoryContainer();

				//workoutCategoryContainer.CategoryId = categoryGroup.Key;
				workoutCategoryContainer.CategoryName = categoryGroup.FirstOrDefault().TrainingCategory.Name;
				workoutCategoryContainer.LiveWorkouts = categoryGroup.ToList();
				upcomingWorkoutContainer.WorkoutCategoryGroups.Add(workoutCategoryContainer);
			}

			return upcomingWorkoutContainer;
		}

		public async Task<IEnumerable<LiveWorkout>> GetTop(int numberOfWorkouts)
		{
			var now = DateTime.UtcNow;
			var twentyFrourHoursInHistory = now.Subtract(new TimeSpan(0, 0, 0, 0));

			var allCandidatesQuery =
				_liveWorkoutRepository.Vault
					.Where(a => a.EndTime <= now)
					.OrderByDescending(a => a.SubscriberCount)
					.OrderByDescending(a => a.StartTime)
					//&& a.StartTime > twentyFrourHoursInHistory //TODO:uncomment
					.Take(numberOfWorkouts);
			// .OrderByDescending(a => a.SubscriberCount);

			return await allCandidatesQuery.ToListAsync();
		}

		public async Task<IEnumerable<LiveWorkout>> GetTopByCategory(int numberOfWorkouts, long categoryId)
		{
			var now = DateTime.UtcNow;
			var twentyFrourHoursInHistory = now.Subtract(new TimeSpan(0, 0, 0, 0));

			var allCandidatesQuery =
				_liveWorkoutRepository.Vault.Where(a => a.EndTime <= now
														//&& a.StartTime > twentyFrourHoursInHistory
														&& a.TrainingCategoryId == categoryId)
					.OrderByDescending(a => a.SubscriberCount)
					.OrderBy(a => a.StartTime)
					.Take(numberOfWorkouts);

			return await allCandidatesQuery.ToListAsync();
		}

		public async Task<UpcomingWorkoutsContainer> GetHomeDataForSignedInUser(string userId)
		{
			long? subscriberId = await GetSubscriberId(userId);

			var availableWorkouts = await GetAvailableWorkouts(subscriberId);
			var upcomingWorkoutsContainer = await GetUpcomingWorkouts(subscriberId);

			upcomingWorkoutsContainer.AllAvailableWorkoutsContainer = new WorkoutCategoryContainer
			{
				CategoryName = "Available Workouts",
				LiveWorkouts = availableWorkouts.ToList()
			};

			var cards = await GetFirst20UpcomingLiveWorkouts(subscriberId);
			upcomingWorkoutsContainer.Cards = cards;

			return upcomingWorkoutsContainer;
		}

		private async Task<IEnumerable<LiveWorkout>> GetAvailableWorkouts(long? subscriberId, int pageId = 1)
		{
			var currentTime = DateTime.UtcNow;
			var candidates =
				await
					_liveWorkoutRepository.GetMultiplePaginated(a => a.EndTime > GetValidEndTime(), a => a.StartTime, new string[] { "Trainer.ApplicationUser", "PriceTier" }, pageId, 20);

			return HydrateWorkoutProperties(subscriberId, candidates.Results.ToList());
		}

		private DateTime GetValidEndTime()
		{
			return DateTime.Now.AddHours(-24 * 30);
		}

		public async Task<IEnumerable<LiveWorkout>> GetUpcomingWorkoutsByTrainer(long trainerId, long? subscriberId = null)
		{
			var currentTime = DateTime.UtcNow;
			var candidates =
				await
					_liveWorkoutRepository.Vault
					.Include(a => a.Trainer.ApplicationUser)
					.Include(a => a.PriceTier)
					.Where(a => a.TrainerId == trainerId && a.EndTime > currentTime)
						.OrderBy(a => a.StartTime)
						.ToListAsync();
			return HydrateWorkoutProperties(subscriberId, candidates);
		}

        public async Task<IEnumerable<LiveWorkout>> GetTrainersUnpublishedVideos(long trainerId)
        {
            var candidates =
                await
                    _liveWorkoutRepository.Vault
                    .Include(a => a.Trainer.ApplicationUser)
                    .Include(a => a.PriceTier)
                    .Where(a => a.TrainerId == trainerId
                     //&& a.WorkoutType == WorkoutType.Recorded 
                     && a.WorkoutType == 2
                    && a.IsArchived == true 
                    && a.PublishDateTime > DateTime.UtcNow)
                        .OrderBy(a => a.StartTime)
                        .ToListAsync();

            return candidates;
        }


        public async Task<IEnumerable<LiveWorkout>> GetUpcomingVideosByCategory(long categoryId,
			long? subscriberId = null)
		{
			var currentTime = DateTime.UtcNow;
			var candidates =
				await
					_liveWorkoutRepository.Vault
						.Include(a => a.LiveWorkoutSubscribers)
						.Include(a => a.Trainer.ApplicationUser)
						.Include(a => a.PriceTier)
						.Where(a => a.TrainingCategoryId == categoryId && a.StartTime > currentTime && a.IsArchived == false)
						.OrderBy(a => a.StartTime)
						.ToListAsync();

			return HydrateWorkoutProperties(subscriberId, candidates);
		}

		private static IEnumerable<LiveWorkout> HydrateWorkoutProperties(long? subscriberId, List<LiveWorkout> candidates)
		{
			foreach (var candidate in candidates)
			{
				HydrateNotMappedProperties(subscriberId, candidate);
			}

			return candidates;
		}

		public async Task<IEnumerable<LiveWorkout>> GetLiveVideosByCategory(long categoryId, long? subscriberId = null)
		{
			var currentTime = DateTime.UtcNow;
			var candidates =
				await
					_liveWorkoutRepository.Vault
						.Include(a => a.LiveWorkoutSubscribers)
						.Include(a => a.Trainer.ApplicationUser)
						.Include(a => a.Trainer.TrainerSubscribers)
						.Include(a => a.PriceTier)
						.Include(a => a.LiveWorkoutFavors)
						.Where(
							a =>
								a.TrainingCategoryId == categoryId && a.StartTime < currentTime &&
								a.EndTime > currentTime && a.IsArchived == false)
						.OrderBy(a => a.StartTime)
						.ToListAsync();


			foreach (var candidate in candidates)
			{
				HydrateNotMappedProperties(subscriberId, candidate);
			}

			return candidates;
		}

		public async Task LikeWorkout(long liveWorkoutId, long subscriberId)
		{
			var alreadyLiked =
				await _liveworkoutFavorRepository.GetFirstOrDefault(
					a => a.SubscriberId == subscriberId && a.LiveWorkoutId == liveWorkoutId);
			if (alreadyLiked.IsNull())
			{
				var liveWorkout = await _liveWorkoutRepository.GetById(liveWorkoutId);
				liveWorkout.LikeCount = liveWorkout.LikeCount + 1;
				await _liveWorkoutRepository.Save();
				await
					_liveworkoutFavorRepository.Add(new LiveWorkoutFavor
					{
						SubscriberId = subscriberId,
						LiveWorkoutId = liveWorkoutId
					});

				var subscriber = await _subscriberService.Get(subscriberId);
				var trainer = await _trainerService.Get(liveWorkout.TrainerId);

				await _notificationsService.SendUserLikedWorkoutNotificaiton(new WorkoutLikedNotificationModel { LiveWorkout = liveWorkout, Subscriber = subscriber, Trainer = trainer });
			}
		}

		public async Task<IEnumerable<LiveWorkout>> Search(string searchText)
		{
			var candidates =
				await
					_liveWorkoutRepository.Vault.Include(a => a.Trainer.ApplicationUser)
						.Where(a => a.Title.Contains(searchText) || a.Trainer.ApplicationUser.Name.Contains(searchText) && a.IsArchived == false)
						.OrderBy(a => a.StartTime)
						.Take(20)
						.ToListAsync();

			foreach (var candidate in candidates)
			{
				candidate.TrainerName = candidate.Trainer.ApplicationUser.Name;
				candidate.TrainerProfileImageUrl = candidate.Trainer.ApplicationUser.ProfileImageUrl;
			}
			return candidates;
		}

		private async Task ValidateTime(DateTime eventStartTimeInUtc, DateTime eventEndTimeInUtc, long trainerId)
		{
			if (eventStartTimeInUtc < DateTime.UtcNow)
				throw new BusinessException("Start time cannot be in the past");

			var thirtyMinsFromExpectedStartTime = eventStartTimeInUtc.AddMinutes(30);
			var thirtyMinsFromExpectedEndTime = eventEndTimeInUtc.AddMinutes(30);
			var overlappingItems = await _liveWorkoutRepository.Vault.Where(
				a => ((a.EndTime >= eventStartTimeInUtc && a.StartTime <= eventStartTimeInUtc)
				|| (a.StartTime <= eventEndTimeInUtc && a.EndTime >= eventEndTimeInUtc)) && a.TrainerId == trainerId).ToListAsync();


			if (overlappingItems.Count > 0)
				throw new BusinessException("There are overlapping workouts! Please review your scheduled workouts and try again!");
			//TODO :Skipped the checks for conflicting workouts for now
		}

		private async Task<TrainingCategory> ValidateAndGetCategory(long trainingCategoryId)
		{
			var trainingCategory = await _trainingCategoryRepository.GetById(trainingCategoryId);
			if (trainingCategory.IsNull())
			{
				throw new BusinessException("Invalid training category");
			}
			return trainingCategory;
		}

		private static void HydrateNotMappedProperties(long? subscriberId, LiveWorkout candidate)
		{

			candidate.IsSubscribed = candidate.IsAlreadySubsribed(subscriberId);
			candidate.IsLiked = candidate.IsAlreadyLiked(subscriberId);
			candidate.IsFollowingTrainer = candidate.IsAleadyFollowingTrainer(subscriberId);
			candidate.TrainerName = candidate.Trainer.ApplicationUser.Name;
			candidate.TrainerProfileImageUrl = candidate.Trainer.ApplicationUser.ProfileImageUrl;
			candidate.IsArchived = candidate.EndTime.AddHours(24 * 30) < DateTime.UtcNow;

    //TODO -check the hard codede 30 values above
		}

		private async Task UpdateLiveworkoutCount(TrainingCategory trainingCategory)
		{
			trainingCategory.LiveWorkoutCount = trainingCategory.LiveWorkoutCount + 1;
			await _trainingCategoryRepository.Save();
		}

		private async Task<List<LiveWorkout>> GetFirst20UpcomingLiveWorkouts(long? subscriberId)
		{
			var currentTime = DateTime.UtcNow;
			var allCandidates =
				await
					_liveWorkoutRepository.Vault
						.Include(a => a.TrainingCategory)
						.Include(a => a.LiveWorkoutSubscribers)
						.Include(a => a.Trainer.ApplicationUser)
						.Include(a => a.PriceTier)
						.Include(a => a.Trainer.TrainerSubscribers)
						.Where(a => a.EndTime > currentTime && a.IsArchived == false).OrderBy(a => a.StartTime).Take(50)
						.ToListAsync();

			foreach (var candidate in allCandidates)
			{
				HydrateNotMappedProperties(subscriberId, candidate);
			}


			return allCandidates;
		}

		private string GetWowzaRegion(string timezone)
		{
			switch (timezone)
			{
				case "Asia/Colombo":
					return "asia_pacific_india";
				case "Australia/Melbourne":
					return "asia_pacific_australia";

				default:
					return "asia_pacific_australia";
			}
		}

		public async Task<LiveWorkout> GetLiveWorkoutByStreamId(string streamId)
		{
			return await _liveWorkoutRepository.GetFirstOrDefault(a => a.StreamId == streamId);
		}

		public Task<UpcomingWorkoutsContainer> GetScheduledWorkouts(int pageId)
		{
			throw new NotImplementedException();
		}

		public async Task<PaginatedResult<LiveWorkout>> GetAvailableWorkoutsPaginated(string userId, int pageId)
		{
			long? subscriberId = await GetSubscriberId(userId);

			var currentTime = DateTime.UtcNow;
			var candidates =
				await
					_liveWorkoutRepository.GetMultiplePaginated(
                       a => a.EndTime > GetValidEndTime() 
                    && a.EndTime < currentTime 
                    && a.IsArchived == false,
                    a => a.StartTime, new string[] { "Trainer.ApplicationUser", "PriceTier" }, pageId, 20);

			var hydratedResults = HydrateWorkoutProperties(subscriberId, candidates.Results.ToList());
			candidates.Results = hydratedResults;
			return candidates;
		}

		public async Task<PaginatedResult<LiveWorkout>> GetAvailableWorkoutsByCategoryPaginated(string userId, long categoryId, int pageId)
		{
			long? subscriberId = await GetSubscriberId(userId);

			var currentTime = DateTime.UtcNow;
			var candidates =
				await
					_liveWorkoutRepository.GetMultiplePaginated(
                        a => a.EndTime > GetValidEndTime() 
                    && a.EndTime < currentTime 
                    && a.TrainingCategoryId == categoryId
                    && a.IsArchived == false
                    , a => a.StartTime, new string[] { "Trainer.ApplicationUser", "PriceTier" }, pageId, 20);

			var hydratedResults = HydrateWorkoutProperties(subscriberId, candidates.Results.ToList());
			candidates.Results = hydratedResults;
			return candidates;
		}

		private async Task<long?> GetSubscriberId(string userId)
		{
			var subscriber = await _subscriberService.GetByUserId(userId);
			var subscriberId = subscriber.IsNotNull() ? subscriber.Id : (long?)null;
			return subscriberId;
		}

		public async Task<PaginatedResult<LiveWorkout>> GetScheduledWorkoutsByCategoryPaginated(string userId, long categoryId, int pageId)
		{
			long? subscriberId = await GetSubscriberId(userId);

			var currentTime = DateTime.UtcNow;
			var candidates =
				await
					_liveWorkoutRepository.GetMultiplePaginated(
                        a => a.StartTime > currentTime 
                        && a.TrainingCategoryId == categoryId
                        && a.IsArchived == false
                        , a => a.StartTime, new string[] { "Trainer.ApplicationUser", "PriceTier" }, pageId, 20);

			var hydratedResults = HydrateWorkoutProperties(subscriberId, candidates.Results.ToList());
			candidates.Results = hydratedResults;
			return candidates;
		}

    }


}