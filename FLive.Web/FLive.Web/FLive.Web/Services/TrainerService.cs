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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;


namespace FLive.Web.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<TrainerFollower> _trainerFollowerRepository;
        private readonly IGenericRepository<TrainerSubsriber> _trainerSubscriberRepository;
        private readonly IGenericRepository<TrainingCategory> _trainingCategoryRepository;
        private readonly IGenericRepository<TrainerTrainingCategory> _trainerTrainingCategory;
        private IGenericRepository<LiveWorkout> _liveWorkoutRepository;

        public TrainerService(IGenericRepository<Trainer> trainerRepository ,
            IGenericRepository<TrainerFollower> trainerFollowerRepository ,
            IGenericRepository<TrainerSubsriber> trainerSubscriberRepository,
            IGenericRepository<TrainingCategory> trainingCategoryRepository,
            IGenericRepository<TrainerTrainingCategory> trainerTrainingCategory , 
            IGenericRepository<LiveWorkout> liveWorkoutRepository )
        {
            _trainerRepository = trainerRepository;
            _trainerFollowerRepository = trainerFollowerRepository;
            _trainerSubscriberRepository = trainerSubscriberRepository;
            _trainingCategoryRepository = trainingCategoryRepository;
            _trainerTrainingCategory = trainerTrainingCategory;
            _liveWorkoutRepository = liveWorkoutRepository;

        }
     
        public async Task<Trainer> Get(long userId, long? subscriberId = null)
        {
         var candidate =  await _trainerRepository.Vault
                .Include(a => a.ApplicationUser)
                .Include(a=>a.TrainerSubscribers)
                .FirstOrDefaultAsync(a => a.Id == userId);

            return HydrateProperties(candidate, subscriberId);
        }

        private  Trainer HydrateProperties(Trainer trainer, long? subscriberId = null)
        {
            trainer.IsSubscribed = trainer.IsAlreadySubsribed(subscriberId);
            return trainer;
        }
        public async Task<Trainer> GetWithSubscribers(long userId)
        {
            return await _trainerRepository.Vault
                   .Include(a => a.ApplicationUser)
                   .Include(a=>a.TrainerSubscribers).ThenInclude(a=>a.Subscriber.ApplicationUser)
                   .FirstOrDefaultAsync(a => a.Id == userId);

        }

        public async Task<Trainer> GetByUserId(string userId)
        {
            return await _trainerRepository.Vault
                .Include(a=>a.ApplicationUser)
                .FirstOrDefaultAsync( a => a.ApplicationUser.Id == userId);
        }

     

        public async Task<IEnumerable<Trainer>> GetAll()
        {
            return await _trainerRepository.GetAll();
        }

        public async Task<IEnumerable<Trainer>> GetSubscribedTrainers(long subscriberId)
        {
            var items = await _trainerSubscriberRepository.GetMultiple(a => a.SubscriberId == subscriberId , new string[] { "Trainer", "Trainer.ApplicationUser" });
            return items.Select(a => a.Trainer);
        }

        public async Task Follow(long trainerId , long userId)
        {
            var trainer = await Get(trainerId);
            trainer.LikeCount = trainer.LikeCount + 1;
           await _trainerFollowerRepository.Add(new TrainerFollower {TrainerId = trainerId, SubscriberId = userId });
            
        }

        public async Task Subscribe(long trainerId, long subscriberId)
        {
            var trainer = await Get(trainerId);
            var trainerSubscriber = await _trainerSubscriberRepository.GetFirstOrDefault(a => a.SubscriberId == subscriberId && a.TrainerId == trainerId);
            if(trainerSubscriber == null)
                await _trainerSubscriberRepository.Add(new TrainerSubsriber { TrainerId = trainerId, SubscriberId = subscriberId });
        }

        public async Task UnSubscribe(long trainerId, long subscriberId)
        {
            var trainerSubscriber = await _trainerSubscriberRepository.GetFirstOrDefault(a => a.SubscriberId == subscriberId && a.TrainerId == trainerId);
            await _trainerSubscriberRepository.Remove(trainerSubscriber);
        }

        public async Task CreateTrainer(ApplicationUser applicationUser , string deviceToken)
        {
            await _trainerRepository.Add(new Trainer {ApplicationUser = applicationUser});
          
        }

        public async Task<Trainer> GetTrainerByAspnetUserId(string userId)
        {
            return await _trainerRepository.Vault
                .Include(a=>a.TrainerTrainingCategories)
                .Include(a=>a.TrainerTrainingGoals)
                .Include(a=>a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.ApplicationUser.Id == userId);
        }

        public async Task<TrainerViewModel> GetTrainerProfile(string userId, long? loggedInUserId=null)
        {


            var trainer= await _trainerRepository.Vault
              .Include(a => a.TrainerTrainingCategories)
              .Include(a => a.TrainerTrainingGoals)
              .Include(a => a.ApplicationUser)
              .Include(a=>a.LiveWorkouts)
              .Include(a=>a.TrainerSubscribers)
              .FirstOrDefaultAsync(a => a.ApplicationUser.Id == userId);
            
            if (trainer.IsNull())
                return null;
            return ToTrainerViewModel(trainer,loggedInUserId);
        }

        private static TrainerViewModel ToTrainerViewModel(Trainer trainer , long? loggedInUserId=null)
        {

            var pastWorkouts = trainer.PastLiveWorkouts.ToList();
            var futureWorkouts =  trainer.FutureLiveWorkouts.ToList();

            return new TrainerViewModel
            {
                AreasOfSpcialisation = trainer.TrainerTrainingCategories.Select(a => a.TrainingCategoryId).ToArray(),
                TrainingGoals = trainer.TrainerTrainingGoals.Select(a => a.TrainingGoalId).ToArray(),
                LevelOfCompetency = trainer.LevelOfCompetency,
                Bio = trainer.Bio,
                LikeCount = trainer.LikeCount,
                VideoCount = trainer.LiveWorkouts.Count,
                FollowerCount = trainer.TrainerSubscribers.Count,
                LatestWorkout = pastWorkouts.Where(a=>a.StartTime < DateTime.UtcNow).OrderByDescending(a=>a.StartTime).FirstOrDefault(),
                PastWorkouts = pastWorkouts.Where(a=>a.EndTime < DateTime.UtcNow).OrderByDescending(a => a.StartTime).ToList(),
                UpcomingWorkouts = futureWorkouts,
                Name = trainer.ApplicationUser.Name,
                IsFollowingTrainer = trainer.IsAleadyFollowingTrainer(loggedInUserId),
                BSB = trainer.BSB,
                AccountNumber = trainer.AccountNumber,
                AddressLine1 = trainer.AddressLine1,
                AddressLine2 = trainer.AddressLine2,
                PostCode = trainer.PostCode,
                Suburb = trainer.Suburb,
                Country = trainer.Country,
                Id = trainer.Id,
                StripeUserId =  trainer.StripeUserId
            };
        }

        public async Task<IEnumerable<TrainerSearchResultModel>> Search(string searchText)
        {
            var candidates =  await _trainerRepository.Vault.Include(a => a.ApplicationUser)
                .Where(a => a.ApplicationUser.Name.Contains(searchText)).ToListAsync();

            var searchResults  = new List<TrainerSearchResultModel>();
            foreach (var candidate in candidates)
            {
                searchResults.Add(new TrainerSearchResultModel {Name = candidate.ApplicationUser.Name , Bio = candidate.Bio , ProfileImageUrl =candidate.ApplicationUser.ProfileImageUrl , TrainerId=candidate.Id});
            }

            return searchResults;

        }

        public async Task<IEnumerable<Trainer>> GetTrainersFromMyLocationByDistance(LocationDataViewModel locationData)
        {

            //Ok this is because of a bad mistake done during the intial requiremenets stage. 
            
            var candidates =
                await
                    _trainerRepository.Context.Set<ApplicationUser>().FromSql($"dbo.getTrainersFromMyLocationByDistance {locationData.Latitude}, {locationData.Longitude}, {locationData.Distance}").ToListAsync();

            string[] userIds = candidates.Select(a => a.Id).ToArray();


            var currentTime = DateTime.UtcNow;
            var liveworkouts = await _liveWorkoutRepository.Vault.Include(a => a.LiveWorkoutSubscribers)
                .Include(a => a.Trainer.ApplicationUser)
                .Where(a => a.StartTime < currentTime && a.EndTime > currentTime)
                .ToListAsync();

            string[] userIdsFromLiveWorkouts = liveworkouts.Select(a => a.Trainer.ApplicationUser.Id).ToArray();

            var liveTrainers = userIds.Intersect(userIdsFromLiveWorkouts);

            var trainers = await _trainerRepository.Vault.Include(a => a.ApplicationUser)
                .Where(a => liveTrainers.Contains(a.ApplicationUser.Id)).ToListAsync();

            var returnables = new List<Trainer>();
            foreach (var trainer in trainers)
            {
                var trainerTrimmed = new Trainer
                {
                    ApplicationUser = trainer.ApplicationUser,
                    Id = trainer.Id,
                    CurrentLiveWorkout = trainer.LiveWorkouts.FirstOrDefault(a => a.StartTime < currentTime && a.EndTime > currentTime)
                };
                returnables.Add(trainerTrimmed);
            }
            return returnables;
        }

        public async Task UpdateStripeToken(long trainerId, string stripeToken,string stripeAuthResult)
        {
            var trainer = await _trainerRepository.GetById(trainerId);
            trainer.StripeUserId = stripeToken;
            trainer.SripeAuthResult = stripeAuthResult;
            await _trainerRepository.Save();
        }

        public async Task UpdateTrainer(ProfileViewModel profilePostViewModel)
        {
            var trainer =
                await GetTrainerByAspnetUserId(profilePostViewModel.UserId);
            if (trainer.IsNull())
                return;
            trainer.Bio = profilePostViewModel.Trainer.Bio;
            trainer.LevelOfCompetency = profilePostViewModel.Trainer.LevelOfCompetency;
            trainer.BSB = profilePostViewModel.Trainer.BSB;
            trainer.AccountNumber = profilePostViewModel.Trainer.AccountNumber;

            trainer.AddressLine1 = profilePostViewModel.Trainer.AddressLine1;
            trainer.AddressLine2 = profilePostViewModel.Trainer.AddressLine2;
            trainer.Suburb = profilePostViewModel.Trainer.Suburb;
            trainer.PostCode = profilePostViewModel.Trainer.PostCode;
            trainer.Country = profilePostViewModel.Trainer.Country;

            UpdateTrainingCategories(profilePostViewModel, trainer);
            UpdateTrainingGoals(profilePostViewModel, trainer);

            await _trainerRepository.Save();
        }

        private void UpdateTrainingCategories(ProfileViewModel profilePostViewModel, Trainer trainer)
        {

            var trainerTrainingCategories = new List<TrainerTrainingCategory>();
            foreach (var areaId in profilePostViewModel.Trainer.AreasOfSpcialisation)
            {
                if (trainer.TrainerTrainingCategories.All(a => a.TrainingCategoryId != areaId))
                {
                    trainerTrainingCategories.Add(new TrainerTrainingCategory
                    {
                        TrainerId = trainer.Id,
                        TrainingCategoryId = areaId
                    });
                }
            }
            foreach (var category in trainerTrainingCategories)
            {
                trainer.TrainerTrainingCategories.Add(category);

            }
        }

        private void UpdateTrainingGoals(ProfileViewModel profilePostViewModel, Trainer trainer)
        {

            var trainerTrainingGoals = new List<TrainerTrainingGoal>();
            foreach (var goal in profilePostViewModel.Trainer.TrainingGoals)
            {
                if (trainer.TrainerTrainingGoals.All(a => a.TrainingGoalId != goal))
                {
                    trainerTrainingGoals.Add(new TrainerTrainingGoal
                    {
                        TrainerId = trainer.Id,
                        TrainingGoalId = goal
                    });
                }
            }

            foreach (var goal in trainerTrainingGoals)
            {
                trainer.TrainerTrainingGoals.Add(goal);

            }

            
        }


        public async Task<IEnumerable<Trainer>> GetTrendingTrainers()
        {
            return await _trainerRepository.Vault.OrderByDescending(a => a.LikeCount).Include(a=>a.ApplicationUser).Take(20).ToListAsync();
        }
    }
}