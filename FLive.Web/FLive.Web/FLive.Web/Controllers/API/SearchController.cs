

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Data;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Repositories;
using FLive.Web.Services;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FLive.Web.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ILiveWorkoutService _liveWorkoutService;
        private readonly ITrainerService _trainerService;

        public SearchController(ILiveWorkoutService liveWorkoutService, ITrainerService trainerService)
        {
            _liveWorkoutService = liveWorkoutService;
            _trainerService = trainerService;
        }

        [HttpGet("{searchText}")]
        public async Task<SearchResultsContainer> Get(string searchText)
        {
            var workouts = await _liveWorkoutService.Search(searchText);
            var trainers = await _trainerService.Search(searchText);
			//var trainers = await _trainerService.Search(searchText);
			return new SearchResultsContainer { LiveWorkouts = workouts, Trainers = trainers };
        }

    }
}