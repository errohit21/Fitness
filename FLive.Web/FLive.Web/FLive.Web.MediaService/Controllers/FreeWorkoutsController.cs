using FLive.Web.MediaService.Helpers;
using FLive.Web.MediaService.Models.flive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLive.Web.MediaService.Controllers
{
    [Authorize]
    [AdminModuleFilter]
    public class FreeWorkoutsController : Controller
    {
        // GET: FreeWorkouts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchFreeWorkoutList(string searchText)
        {
            var liveWorkouts = LiveWorkout.GetFreeLiveWorkouts(searchText);
            return View(liveWorkouts);
        }

        public ActionResult SearchAddNewFreeWorkoutList(string searchText)
        {
            var liveWorkouts = LiveWorkout.GetPaidLiveWorkouts(searchText);
            return View(liveWorkouts);
        }
    }
}