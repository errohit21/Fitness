using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Repositories;
using FLive.Web.Shared;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace FLive.Web.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly IGenericRepository<LiveWorkoutSubscriber> _liveworkoutSubscriberGenericRepository;
        private readonly IGenericRepository<Subscriber> _subscriberRepository;
        private readonly IGenericRepository<TrainerFavor> _trainerFavorRepository;
        private readonly StripeConfig _stripeConfig;
        private readonly IGenericRepository<TrainerFollower> _tranerSubscriberRepository;

        public SubscriberService(IGenericRepository<Subscriber> trainerRepository,
            IGenericRepository<LiveWorkoutSubscriber> liveworkoutSubscriberGenericRepository , IOptions<StripeConfig> stripeConfig, IGenericRepository<TrainerFollower> tranerSubscriberRepository)
        {
            _subscriberRepository = trainerRepository;
            _liveworkoutSubscriberGenericRepository = liveworkoutSubscriberGenericRepository;
            _stripeConfig = stripeConfig.Value;
            _tranerSubscriberRepository = tranerSubscriberRepository;
        }

        public async Task<Subscriber> GetSubscriberByAspnetUserId(string userId)
        {

            return await _subscriberRepository.Vault
                .Include(a => a.SubscriberTrainingGoals)
                .Include(a => a.Workouts)
                .Include(a=>a.Following)
                .FirstOrDefaultAsync(a => a.ApplicationUser.Id == userId);


        }

        public async Task<Subscriber> Get(long subscriberId)
        {
            return await _subscriberRepository.Vault
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == subscriberId);
        }

        public async Task UpdateSubscriber(ProfileViewModel profilePostViewModel)
        {
            var subscriber =
                await GetSubscriberByAspnetUserId(profilePostViewModel.UserId);

            if (profilePostViewModel.User.LevelOfCompetency.IsNotNull())
                subscriber.LevelOfCompetency = profilePostViewModel.User.LevelOfCompetency;
            if (!string.IsNullOrEmpty(profilePostViewModel.User.StripeCustomerId) && profilePostViewModel.User.StripeCustomerId != subscriber.StripeCustomerId)
            {
                var stripeCustomerId = await GetStripeCustomerId(profilePostViewModel.User.StripeCustomerId, subscriber.ApplicationUser.Email);
                if (!string.IsNullOrEmpty(stripeCustomerId))
                    subscriber.StripeCustomerId = stripeCustomerId;
                subscriber.Currency = GetCurrencyFromTimezone(profilePostViewModel.Timezone);
            }
            UpdateTrainingGoals(profilePostViewModel, subscriber);

            await _subscriberRepository.Save();
        }


        private async Task<string> GetStripeCustomerId(string token,string email)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_stripeConfig.SecretKey}");

            var url =
                $"https://api.stripe.com/v1/customers?email={email}&source={token}";
            var result =
          await
                httpClient.PostAsync(url, new StringContent(""));
            var jsonResult = await result.Content.ReadAsStringAsync();

            dynamic d = JObject.Parse(jsonResult);
            return d.id;

        }

        public async Task<SubscriberModal> GetSubscriberProfile(string userId)
        {
            var subscriber = await GetSubscriberByAspnetUserId(userId);
            if (subscriber.IsNull())
                return null;
            var workouts = await GetWorkouts(subscriber.Id);
            // var workoutsMasssaged = LiveWorkoutService.HydrateWorkoutProperties(subscriber.Id ,workouts.ToList());
            var followingCount = await _tranerSubscriberRepository.Vault.Where(a => a.SubscriberId == subscriber.Id).CountAsync();
            var upcomingWorkouts = workouts.Where(a => a.StartTime > DateTime.Now);
            var latestWorkout = workouts.Where(a => a.StartTime < DateTime.UtcNow).OrderByDescending(a => a.StartTime).FirstOrDefault();
            var pastWorkouts = workouts.Where(a => a.EndTime < DateTime.UtcNow).OrderByDescending(a => a.StartTime).ToList();

            return new SubscriberModal
            {
                TrainingGoals = subscriber.SubscriberTrainingGoals.Select(a => a.TrainingGoalId).ToArray(),
                LevelOfCompetency = subscriber.LevelOfCompetency,
                WorkoutsCount = subscriber.WorkoutsCount,
                FollowingCount = followingCount,
                UpcomingWorkouts = upcomingWorkouts,
                PastWorkouts = pastWorkouts,
                StripeCustomerId = subscriber.StripeCustomerId,
                LatestWorkout = latestWorkout,
                Id = subscriber.Id
            };
        }

        public async Task<Subscriber> GetByUserId(string subscriberId)
        {
            return
                await
                    _subscriberRepository.Vault.Include(a => a.ApplicationUser)
                        .FirstOrDefaultAsync(a => a.ApplicationUser.Id == subscriberId);
        }

        public async Task<IEnumerable<Subscriber>> GetAll()
        {
            return await _subscriberRepository.GetAll();
        }

        public async Task CreateSubscriber(ApplicationUser applicationUser, string deviceToken)
        {
            await _subscriberRepository.Add(new Subscriber {ApplicationUser = applicationUser});
        }

        private string GetCurrencyFromTimezone(string timezone)
        {
            if (timezone.Contains("Australia"))
                return "AUD";
            if (timezone.Contains("USA"))
                return "USD";
            if (timezone.Contains("Asia"))
                return "LKR";
            throw new BusinessException("Could not derive currency from the timezone ");
        }

        private async Task<List<LiveWorkout>> GetWorkouts(long subscriberId)
        {
            var upcomingWorkouts =
                await
                    _liveworkoutSubscriberGenericRepository.Vault
                        .Include(a => a.LiveWorkout)
                        .Where(a=> a.SubscriberId == subscriberId)
                        .ToListAsync();
            var returnable =  upcomingWorkouts.Select(a=>a.LiveWorkout).ToList();
            return returnable;
        }

        private void UpdateTrainingGoals(ProfileViewModel profilePostViewModel, Subscriber subscriber)
        {
            var subscriberTrainingGoals = new List<SubscriberTrainingGoal>();
            foreach (var goal in profilePostViewModel.User.TrainingGoals)
            {
                if (subscriber.SubscriberTrainingGoals.All(a => a.TrainingGoalId != goal))
                {
                    subscriberTrainingGoals.Add(new SubscriberTrainingGoal
                    {
                        SubscriberId = subscriber.Id,
                        TrainingGoalId = goal
                    });
                }
            }

            foreach (var item in subscriberTrainingGoals)
            {
                subscriber.SubscriberTrainingGoals.Add(item);

            }
        }

        public async Task Follow(long subscriberId, long trainerId)
        {
            throw new NotImplementedException();
        }
    }
}