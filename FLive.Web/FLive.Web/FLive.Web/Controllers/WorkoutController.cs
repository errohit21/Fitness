using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FLive.Web.Services;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace FLive.Web.Controllers
{
    //[Authorize]
    [Route("Workout")]
    public class WorkoutController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly StripeConfig _stripeConfig;
        private readonly ILogger _logger;
        private ILiveWorkoutService _liveWorkoutService;

        public WorkoutController(ILiveWorkoutService liveWorkoutService, IOptions<StripeConfig> stripeConfig)
        {
            _liveWorkoutService = liveWorkoutService;
            _stripeConfig = stripeConfig.Value;
            //_logger = logger;
        }

        #region commented code on 12 dec 2017

        //[Route("{workoutId}")]
        //public async Task<IActionResult> Index(long workoutId)
        //{

        //    var workout = await _liveWorkoutService.Get(workoutId);

        //    if (workout.IsNotNull())
        //    {
        //        ViewData.Add("PreviewImageUrl", workout.PreviewImagelUrl);
        //        ViewData.Add("TrainerName", workout.Trainer.ApplicationUser.Name);
        //        ViewData.Add("ProfileImageUrl", workout.Trainer.ApplicationUser.ProfileImageUrl);
        //        ViewData.Add("Title", workout.Title);
        //        ViewData.Add("WorkoutId", workout.Id);

        //        string easternZoneId = "AUS Eastern Standard Time";
        //        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
        //       var ausTime = TimeZoneInfo.ConvertTime(workout.StartTime, easternZone);

        //        ViewData.Add("WorkoutTime", ausTime.ToString("f"));

        //        return View();
        //    }
        //    else
        //    {
        //        return View("Error");
        //    }


        //}

        #endregion


    }

}
