using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Migrations;
using FLive.Web.Models;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Repositories;
using FLive.Web.Services;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FLive.Web.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IFileUploadRepository _fileUploadRepository;
        private  readonly ILiveWorkoutService _liveWorkoutService;
        private UserManager<ApplicationUser> _userManager;
        private readonly ISubscriberService _subscriberService;
        private readonly ILocationService _locationService;
        private readonly IAzurePushNotificationService _notificationService;
        private readonly IGenericRepository<Transaction> _transactionRepository;

        public TrainerController(ITrainerService trainerService , IFileUploadRepository fileUploadRepository, ILiveWorkoutService liveWorkoutService, UserManager<ApplicationUser> userManager , ISubscriberService subscriberService, ILocationService locationService,IAzurePushNotificationService notificationService, IGenericRepository<Transaction> transactionRepository)
        {
            _trainerService = trainerService;
            _fileUploadRepository = fileUploadRepository;
            _liveWorkoutService = liveWorkoutService;
            _userManager = userManager;
            _subscriberService = subscriberService;
            _locationService = locationService;
            _notificationService = notificationService;
            _transactionRepository = transactionRepository;
        }
        [AllowAnonymous]
        [HttpGet("{trainerId}")]
        public async Task<Trainer> Get(long trainerId)
        {
            var user = await _subscriberService.GetByUserId(User.GetUserId());

            //return await _trainerService.Get(trainerId, user.Id);
            return await _trainerService.Get(trainerId, 187);
        }


        [HttpGet("all")]
        public async Task<IEnumerable<Trainer>> GetAll(string trainerId)
        {
            return await _trainerService.GetAll();
        }

        [AllowAnonymous]
        [HttpPost("nearme")]
        public async Task<IEnumerable<Trainer>> GetNearMe([FromBody]LocationDataViewModel locationDataViewModel)
        {
            var trainers =  await _trainerService.GetTrainersFromMyLocationByDistance(locationDataViewModel);
            return trainers;
        }

        [AllowAnonymous]
        [HttpGet("{trainerId}/upcoming")]
        public async Task<IEnumerable<LiveWorkout>> GetUpcomingWorkoutsByTrainer(long trainerId)
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());
            return await _liveWorkoutService.GetUpcomingWorkoutsByTrainer(trainerId,subscriber.Id);
        }


        [HttpGet("upcoming")]
        public async Task<IEnumerable<LiveWorkout>> GetTrainersUpcomingVideos()
        {
            var trainer = await _trainerService.GetByUserId(User.GetUserId());
            var result=  await _liveWorkoutService.GetUpcomingWorkoutsByTrainer(trainer.Id);
            return result;
        }


        [HttpGet("workouts/unpublished")]
        public async Task<IEnumerable<LiveWorkout>> GetTrainersUnpublishedVideos()
        {
            var trainer = await _trainerService.GetByUserId(User.GetUserId());
            var result = await _liveWorkoutService.GetTrainersUnpublishedVideos(trainer.Id);
            return result;
        }


        [HttpGet("earnings/{page}")]
        public async Task<PaginatedResult<Transaction>> GetTrainerErnings(int page=1)
        {
            var trainer = await _trainerService.GetByUserId(User.GetUserId());
            var result = await _transactionRepository.GetMultiplePaginated(a => a.TrainerId == trainer.Id, a=>a.TranactionDateTime, new string[] { "LiveWorkout", "LiveWorkout.PriceTier"}, page,20);
            return result;
        }

        [AllowAnonymous]
        [HttpGet("trending")]
        public async Task<IEnumerable<Trainer>> GetTrendingTrainers()   
        {
            return await _trainerService.GetTrendingTrainers();
        }

        [HttpPost("{trainerId}/follow")]
        public async Task FollowTrainer(long trainerId)
        {
            var user = await _subscriberService.GetByUserId(User.GetUserId());
            await _trainerService.Follow(trainerId, user.Id);
            await _notificationService.SendFollowNotification(trainerId, user.Id);
        }

        [HttpPost("{trainerId}/subscribe")]
        public async Task SubscribeToTrainer(long trainerId)
        {
            var user = await _subscriberService.GetByUserId(User.GetUserId());
            await _trainerService.Subscribe(trainerId, user.Id);
            await _notificationService.SendFollowNotification(trainerId, user.Id);
        }

        [HttpPost("{trainerId}/unsubscribe")]
        public async Task UnSubscribeFromTrainer(long trainerId)
        {
            var user = await _subscriberService.GetByUserId(User.GetUserId());
            await _trainerService.UnSubscribe(trainerId, user.Id);
            await _notificationService.SendFollowNotification(trainerId, user.Id);
        }

    }
}