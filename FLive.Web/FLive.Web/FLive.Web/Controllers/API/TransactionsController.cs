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

namespace FLive.Web.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly ILiveWorkoutService _liveWorkoutService;
        private UserManager<ApplicationUser> _userManager;
        private readonly ISubscriberService _subscriberService;
        private readonly ILocationService _locationService;
        private readonly IAzurePushNotificationService _notificationService;
        private readonly IGenericRepository<Transaction> _transactionRepository;

        public TransactionsController(ITrainerService trainerService, IFileUploadRepository fileUploadRepository, ILiveWorkoutService liveWorkoutService, UserManager<ApplicationUser> userManager, ISubscriberService subscriberService, ILocationService locationService, IAzurePushNotificationService notificationService, IGenericRepository<Transaction> transactionRepository)
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


        [HttpGet("upcoming")]
        public async Task<IEnumerable<LiveWorkout>> GetTrainersUpcomingVideos()
        {
            var trainer = await _trainerService.GetByUserId(User.GetUserId());
            var result = await _liveWorkoutService.GetUpcomingWorkoutsByTrainer(trainer.Id);
            return result;
        }

        ////[AllowAnonymous]
        //[HttpGet("earnings/{page}")]
        //public async Task<PaginatedResult<Transaction>> GetTrainerErnings(int page = 1)
        //{
        //    var trainer = await _trainerService.GetByUserId(User.GetUserId());
        //    var result = await _transactionRepository.GetMultiplePaginated(a => a.TrainerId == trainer.Id, page, 20);
        //    return result;
        //}


    }
}