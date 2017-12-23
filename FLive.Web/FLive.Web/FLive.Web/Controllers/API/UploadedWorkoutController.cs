using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Repositories;
using FLive.Web.Services;
using FLive.Web.Shared;
using FLive.Web.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Serilog;
using System;
using Serilog.Sinks.Slack;

namespace FLive.Web.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class UploadedWorkoutController : Controller
    {
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly ILiveWorkoutService _liveWorkoutService;
        private readonly ISubscriberService _subscriberService;
        private readonly IConnectionManager _connectionManager;

        public UploadedWorkoutController(ILiveWorkoutService liveWorkoutService, IFileUploadRepository fileUploadRepository , ISubscriberService subscriberService, IConnectionManager connectionManager)
        {
            _liveWorkoutService = liveWorkoutService;
            _fileUploadRepository = fileUploadRepository;
            _subscriberService = subscriberService;
            _connectionManager = connectionManager;

        }

        [HttpGet("{workoutId}")]
        [AllowAnonymous]
        public async Task<LiveWorkout> Get(long workoutId)
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());

            return await _liveWorkoutService.Get(workoutId, subscriber.IsNotNull() ? subscriber.Id :(long?)null);
        }

        [HttpDelete("{workoutId}")]
        public async Task Delete(long workoutId)
        {
            await _liveWorkoutService.Delete(workoutId, User.GetUserId());
        }

        [AllowAnonymous]
        [HttpGet("livenow")]
        public async Task<IEnumerable<LiveWorkout>> GetLiveNow()
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());
            var subscriberId = subscriber.IsNotNull() ? subscriber.Id : (long?)null;
            return await _liveWorkoutService.GetCurrentLiveWorkouts(subscriberId);
        }

        [AllowAnonymous]
        [HttpGet("upcoming/bycategory/{categoryId}")]
        public async Task<IEnumerable<LiveWorkout>> GetWorkoutsUpcomingByCategory(long categoryId)
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());
            var subscriberId = subscriber.IsNotNull() ? subscriber.Id : (long?) null;
            return await _liveWorkoutService.GetUpcomingVideosByCategory(categoryId, subscriberId);
        }

        [AllowAnonymous]
        [HttpGet("livenow/bycategory/{categoryId}")]
        public async Task<IEnumerable<LiveWorkout>> GetLiveWorkoutsByCategory(long categoryId)
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());
            var subscriberId = subscriber.IsNotNull() ? subscriber.Id : (long?)null;
            return await _liveWorkoutService.GetLiveVideosByCategory(categoryId, subscriberId);
        }


        [AllowAnonymous]
        [HttpGet("top/bycategory/{categoryId}")]
        public async Task<IEnumerable<LiveWorkout>> GetTopWorkoutsByCategory(long categoryId)
        {
            return await _liveWorkoutService.GetTopByCategory(3,categoryId);
        }



        [AllowAnonymous]
        [HttpGet("top")]
        public async Task<IEnumerable<LiveWorkout>> GetTopWrokoutsForHome()
        {
            return await _liveWorkoutService.GetTop(3);
        }


        [AllowAnonymous]
        [HttpGet("upcoming")]
        public async Task<UpcomingWorkoutsContainer> GetWorkoutSummary()
        {
            return await _liveWorkoutService.GetUpcomingWorkouts();
        }
       

        [HttpGet("home")]
        public async Task<UpcomingWorkoutsContainer> GetHomeDataForSignedInUser()
        {
            var result = await _liveWorkoutService.GetHomeDataForSignedInUser(User.GetUserId());
            return result;

        }


		[HttpGet("available/all/{pageId}")]
		public async Task<PaginatedResult<LiveWorkout>> GetAvailableWorkoutsPaginated(int pageId=1)
		{
			var result = await _liveWorkoutService.GetAvailableWorkoutsPaginated(User.GetUserId(),pageId);
			return result;
		}

		[HttpGet("available/category/{categoryId}/{pageId}")]
		public async Task<PaginatedResult<LiveWorkout>> GetAvailableWorkoutsByCategoryPaginated(long categoryId , int pageId = 1)
		{
			var result = await _liveWorkoutService.GetAvailableWorkoutsByCategoryPaginated(User.GetUserId(), categoryId, pageId);
			return result;
		}

		[HttpGet("scheduled/category/{categoryId}/{pageId}")]
		public async Task<PaginatedResult<LiveWorkout>> GetScheduledWorkoutsByCategoryPaginated(long categoryId, int pageId = 1)
		{
			var result = await _liveWorkoutService.GetScheduledWorkoutsByCategoryPaginated(User.GetUserId(), categoryId, pageId);
			return result;
		}


		[HttpGet("summarybycategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<IEnumerable<LiveWorkout>> GetSummaryByCategory(long categoryId)
        {
            var result =  await _liveWorkoutService.GetCurrentLiveWorkouts();
            return result;
        }

        [HttpPost]
        public async Task<WorkoutCreateResult> Post([FromBody] LiveEventViewModel liveEvent)
        {
            return await _liveWorkoutService.Create(User.GetUserId(), liveEvent);
        }

        [HttpPost("{liveworkoutId}/previewvideo")]
        [Produces("application/json")]
        [Consumes("multipart/form-data", "application/json-patch+json", "application/json")]
        public async Task<LiveWorkout> UploadPreviewVideo(long liveworkoutId, IFormFile file)
        {
            var uploadedUrl = "";

            var stream = file.OpenReadStream();
            uploadedUrl = await _fileUploadRepository.UploadFileAsBlob(stream, file.FileName, "previewvideos");
            await _liveWorkoutService.SetPreviewVideo(liveworkoutId, uploadedUrl);
            return await _liveWorkoutService.Get(liveworkoutId);

        }

        [HttpPost("{liveworkoutId}/previewimage")]
        [Produces("application/json")]
        [Consumes("multipart/form-data", "application/json-patch+json", "application/json")]
        public async Task<LiveWorkout> UploadPreviewImage(long liveworkoutId, IFormFile file)
        {
            var uploadedUrl = "";

            var stream = file.OpenReadStream();
            uploadedUrl = await _fileUploadRepository.UploadFileAsBlob(stream, file.FileName, "previewimages");
            await _liveWorkoutService.SetPreviewImage(liveworkoutId, uploadedUrl);
            return await _liveWorkoutService.Get(liveworkoutId);

        }

        [HttpPost("{liveWorkoutId}/subscribe")]
        public async Task Subsribe(long liveWorkoutId)
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());
            await _liveWorkoutService.Subscribe(liveWorkoutId, subscriber.Id);
        }

        [HttpPost("{liveWorkoutId}/unsubscribe")]
        public async Task UnSubsribe(long liveWorkoutId)
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());
            await _liveWorkoutService.UnSubscribe(liveWorkoutId, subscriber.Id);
        }


        [HttpPost("{liveWorkoutId}/complete")]
        public async Task CompleteWorkout(long liveWorkoutId)
        {
            await _liveWorkoutService.CompleteWorkout(liveWorkoutId);
        }

        [HttpPost("{liveWorkoutId}/like")]
        public async Task LikeWorkout(long liveWorkoutId)
        {
            var favorer = await _subscriberService.GetByUserId(User.GetUserId());
            await _liveWorkoutService.LikeWorkout(liveWorkoutId, favorer.Id);

            var workout = await _liveWorkoutService.Get(liveWorkoutId);
          
            var likeCountJson =    $@"{{""likeCount"":""{workout.LikeCount}"",""liveWorkoutId"":""{workout.Id}""}}";
            _connectionManager.GetHubContext<WorkoutNotificationsHub>().Clients.Group(liveWorkoutId.ToString()).publishPost(likeCountJson);

        }

        [HttpPost("{liveWorkoutId}/report")]
        public async Task ReportWorkout(long liveWorkoutId)
        {
            var user = await _subscriberService.GetByUserId(User.GetUserId());
            var workout = await _liveWorkoutService.Get(liveWorkoutId);

           var newlogger =  new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .Enrich.FromLogContext()
                 .WriteTo.Slack("https://hooks.slack.com/services/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")
                 .CreateLogger();

            newlogger.Information("Workout: {@Title} by :{@TrainerName} has been reported as inappropriate by:{@Name} on (UTC Time): {@UtcNow}", workout.Title, workout.TrainerName, user.ApplicationUser.Name,DateTime.UtcNow);
        }



        [HttpGet("subscribers")]
        public async Task<IEnumerable<Subscriber>> GetSubscribers(long liveWorkoutId)
        {
            return await _liveWorkoutService.GetSubscribers(liveWorkoutId);
        }


    }
}