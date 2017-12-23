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
    public class TrainersController : Controller
    {
        // GET: Trainers
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult SearchTrainerList(string searchText)
        {
            var trainerList = Trainer.SearchTrainers(searchText);
            return View(trainerList);
        }

        public ActionResult AddEditTrainer(string id)
        {
            var model = Trainer.GetTrainer(id);
            model.LiveWorkouts = LiveWorkout.GetLiveWorkoutsForTrainer(id);
            return View(model);
        }
    }
}