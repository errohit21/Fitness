using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLive.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "API - A collection of Master Data APIs")]
    //[Authorize]
    public class MasterDataController : Controller
    {
        private readonly IMasterDataService _masterDataService;

        public MasterDataController(IMasterDataService masterDataService)
        {

            _masterDataService = masterDataService;
        }

        [HttpGet("GetMasterdataViewModel")]
        public async Task<MasterdataViewModel> Get()
        {
            var competencies = await _masterDataService.GetAllCompetencyLevels();
            var trainingCategories = await _masterDataService.GetAllTrainingCategories();
            var trainingGoals = await _masterDataService.GetAllTrainingGoals();
            var priceTiers = await _masterDataService.GetAllPriceTiers();
            var workoutLengths = await GetWorkoutLengths();
            var countries = await GetCoutries();
			var settings = await GetSettings();

			settings.CountryList = countries;

			var trainerMasterData = new TrainerMasterData
            {
                Competencies = competencies,
                TrainingCategories = trainingCategories,
                TrainingGoals = trainingGoals,
                PriceTiers = priceTiers,
                WorkoutLengths = workoutLengths,
                Instructions = "<h1>Instructions</h1><p>Trainer Instructions would go here</p>"
            };

            var subscriverMasterData = new SubscriberMasterData
            {
                Competencies = competencies,
                TrainingGoals = trainingGoals,
                Instructions = "<h1>Instructions</h1><p>User Instructions would go here</p>"
            };

            return new MasterdataViewModel { Trainer = trainerMasterData, User = subscriverMasterData, Settings = settings};

        }

		private Task<IEnumerable<Country>> GetCoutries()
		{
			return _masterDataService.GetCountries();
		}

		[HttpGet("trainingcategories")]
        public async Task<IEnumerable<TrainingCategory>> GetTrainingCategories()
        {
            return await _masterDataService.GetAllTrainingCategories();
        }

        [HttpGet("traininggoals")]
        public async Task<IEnumerable<TrainingGoal>> GetTrainingGoals()
        {
            return await _masterDataService.GetAllTrainingGoals();
        }

        [HttpGet("pricetiers")]
        public async Task<IEnumerable<PriceTier>> GetPriceTiers()
        {
            return await _masterDataService.GetAllPriceTiers();
        }


        [HttpGet("competencylevels")]
        public async Task<IEnumerable<Competency>> GetCompetencyLevels()
        {
            return await _masterDataService.GetAllCompetencyLevels();
        }

        [HttpGet("competencylevels1")]
        public async Task<IEnumerable<WorkoutLength>> GetLenths()
        {
            return await GetWorkoutLengths();
        }

        private string[] GetCountries()
		{
			return new string[] {"Australia","Brazil","Canada", "China", "France", "Germany", "India","Ireland", "Indonesia", "Italy", "Japan", "Mexico","New Zealand","Poland","Republic of Korea", "Russia", "South Africa","Spain","United Kingdom","United States"
};
		}

		private static async Task<IEnumerable<WorkoutLength>> GetWorkoutLengths()
        {
            var workoutLengths = new List<WorkoutLength>
            {
                new WorkoutLength {Lenght = 15},
                new WorkoutLength {Lenght = 30},
                new WorkoutLength {Lenght = 45},
                new WorkoutLength {Lenght = 60}
            };
            return await Task.FromResult(workoutLengths);
        }

        private async Task<Settings> GetSettings()
        {
            var settings = await _masterDataService.GetSettings();
			settings.Countries =  GetCountries();
			return settings;
        }

    }
}
