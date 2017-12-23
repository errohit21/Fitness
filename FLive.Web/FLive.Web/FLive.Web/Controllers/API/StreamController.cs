using System.Threading.Tasks;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Services;
using Microsoft.AspNetCore.Mvc;
using FLive.Web.Repositories;
using FLive.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FLive.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class StreamController : Controller
    {
        private readonly IStreamManagementService _streamManagementService;
        private readonly ILiveWorkoutService _liveWorkoutService;

        public StreamController( IStreamManagementService streamManagementService, ILiveWorkoutService liveWorkoutService)
        {
            _streamManagementService = streamManagementService;
			_liveWorkoutService = liveWorkoutService;

		}


        [HttpGet("{streamId}")]
        public async Task<string> Get(string streamId)
        {
            var user = User.Identity;
            return await _streamManagementService.Get(streamId);
        }

        [HttpPost("create")]
        public async Task<string> Create([FromBody]CreateStreamViewModel createStreamViewModel) //todo : move this to event controller
        {
            return await _streamManagementService.Create(createStreamViewModel);
        }


        [HttpPost("start/{streamId}")]
        public async Task<string> Start(string streamId)
        {
            return await _streamManagementService.Start(streamId);
        }

        [HttpPost("stop/{streamId}")]
        public async Task<string> Stop(string streamId)
        {
			var liveWorkout = await _liveWorkoutService.GetLiveWorkoutByStreamId(streamId);
			await _liveWorkoutService.CompleteWorkout(liveWorkout.Id);
			return await _streamManagementService.Stop(streamId);

		}


		[HttpPost("delete/{streamId}")]
        public async Task<string> Delete(string streamId)
        {
            return await _streamManagementService.Delete(streamId);
        }

        [HttpGet("all")]
        public async Task<string> GetAll()
        {
            return await _streamManagementService.GetAll();
        }

    }


}
