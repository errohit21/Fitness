using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Repositories;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Management.NotificationHubs;
namespace FLive.Web.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly IGenericRepository<PriceTier> _priceTierRepository;
        private readonly IGenericRepository<TrainingCategory> _trainingCategoryRepository;
        private readonly IGenericRepository<TrainingGoal> _trainingGoalRepository;
        private readonly IGenericRepository<Competency> _competencyRepository;
        private readonly IGenericRepository<Country> _countryRepository;
        private readonly IGenericRepository<WorkoutType> _workoutTypeRepository;
        private readonly IGenericRepository<WorkoutLength> _workoutLengthRepository;

        public SeedDataService(IGenericRepository<PriceTier> priceTierRepository,
            IGenericRepository<TrainingCategory> trainingCategoryRepository, 
            IGenericRepository<TrainingGoal> trainingGoalRepository,
            IGenericRepository<Competency> competencyRepository,
            IGenericRepository<Country> countryRepository,
         IGenericRepository<WorkoutType> workoutTypeRepository,
            IGenericRepository<WorkoutLength> workoutLengthRepository)
        {

			_priceTierRepository = priceTierRepository;
            _trainingCategoryRepository = trainingCategoryRepository;
            _trainingGoalRepository = trainingGoalRepository;
            _competencyRepository = competencyRepository;
			_countryRepository = countryRepository;
            _workoutTypeRepository = workoutTypeRepository;
            _workoutLengthRepository = workoutLengthRepository;

        }

        public async Task SeedData()
        {
            await SeedPriceTiers();
            await SeedTraingGoals();
            await SeedCompetencies();
			await SeedCountries();
            //await SeedWorkoutTypes();
            //await SeedworkOutLengths();

            var trainingCategories = await _trainingCategoryRepository.GetAll();
            if (!trainingCategories.Any())
            {
                //await _trainingCategoryRepository.Add(new TrainingCategory {Name = "Fat Loss"});
                //await _trainingCategoryRepository.Add(new TrainingCategory {Name = "Building Muscle" });
                //await _trainingCategoryRepository.Add(new TrainingCategory {Name = "Cardio" });
                //await _trainingCategoryRepository.Add(new TrainingCategory {Name = "Functional" });
                await _trainingCategoryRepository.Add(new TrainingCategory { Name = "BuildingMuscle" });
                await _trainingCategoryRepository.Add(new TrainingCategory { Name = "Weight Loss" });
                await _trainingCategoryRepository.Add(new TrainingCategory { Name = "Yoga and Core" });
                await _trainingCategoryRepository.Add(new TrainingCategory { Name = "Cardio and Endurance" });
                await _trainingCategoryRepository.Add(new TrainingCategory { Name = "Other" });
            }
        }

        private async Task SeedPriceTiers()
        {
            var priceTiers = await _priceTierRepository.GetAll();
            if (!priceTiers.Any())
            {
                await _priceTierRepository.Add(new PriceTier {Currency = "AUD", Price = 3});
                await _priceTierRepository.Add(new PriceTier {Currency = "AUD", Price = 5});
                await _priceTierRepository.Add(new PriceTier {Currency = "AUD", Price = 10});
                await _priceTierRepository.Add(new PriceTier {Currency = "AUD", Price = 10});
            }
        }

		private async Task SeedCountries()
		{
			var countries = await _countryRepository.GetAll();
			if (!countries.Any())
			{
				await _countryRepository.Add(new Country { Name = "Australia", DialingCode = "+61" });
				await _countryRepository.Add(new Country { Name = "Brazil", DialingCode = "+55" });
				await _countryRepository.Add(new Country { Name = "Canada", DialingCode = "+1" });
				await _countryRepository.Add(new Country { Name = "China", DialingCode = "+86" });
				await _countryRepository.Add(new Country { Name = "France", DialingCode = "+33" });
				await _countryRepository.Add(new Country { Name = "Germany", DialingCode = "+49" });
				await _countryRepository.Add(new Country { Name = "India", DialingCode = "+91" });
				await _countryRepository.Add(new Country { Name = "Ireland", DialingCode = "+353" });
				await _countryRepository.Add(new Country { Name = "Indonesia", DialingCode = "+62" });
				await _countryRepository.Add(new Country { Name = "Italy", DialingCode = "+39" });
				await _countryRepository.Add(new Country { Name = "Japan", DialingCode = "+81" });
				await _countryRepository.Add(new Country { Name = "Mexico", DialingCode = "+52" });
				await _countryRepository.Add(new Country { Name = "New Zealand", DialingCode = "+64" });
				await _countryRepository.Add(new Country { Name = "Poland", DialingCode = "+48" });
				await _countryRepository.Add(new Country { Name = "Republic of Korea", DialingCode = "+82" });
				await _countryRepository.Add(new Country { Name = "Russia", DialingCode = "+7" });
				await _countryRepository.Add(new Country { Name = "South Africa", DialingCode = "+27" });
				await _countryRepository.Add(new Country { Name = "Spain", DialingCode = "+34" });
				await _countryRepository.Add(new Country { Name = "Sri Lanka", DialingCode = "+94" });
				await _countryRepository.Add(new Country { Name = "United Kingdom", DialingCode = "+44" });
				await _countryRepository.Add(new Country { Name = "United States", DialingCode = "+1" });

			}
		}


		private async Task SeedTraingGoals()
        {
            var trainingGoals = await _trainingGoalRepository.GetAll();
            if (!trainingGoals.Any())
            {
                await _trainingGoalRepository.Add(new TrainingGoal {Name = "Get muscle"});
                await _trainingGoalRepository.Add(new TrainingGoal {Name = "Healthy back"});
               
            }
        }

        private async Task SeedCompetencies()
        {
            var competencies = await _competencyRepository.GetAll();
            if (!competencies.Any())
            {
                await _competencyRepository.Add(new Competency { Name = "Beginner" });
                await _competencyRepository.Add(new Competency { Name = "Intermediate" });
                await _competencyRepository.Add(new Competency { Name = "Advanced" });

            }
        }

        private async Task SeedWorkoutTypes()
        {
            var workoutTypes = await _workoutLengthRepository.GetAll();
            if (!workoutTypes.Any())
            {
                await _workoutTypeRepository.Add(new WorkoutType { Type = "Gym" });
                await _workoutTypeRepository.Add(new WorkoutType { Type = "Home-Equipment" });
                await _workoutTypeRepository.Add(new WorkoutType { Type = "Bodyweight" });
                await _workoutTypeRepository.Add(new WorkoutType { Type = "Office" });

            }
        }
        private async Task SeedworkOutLengths()
        {
            var workoutTypes = await _workoutLengthRepository.GetAll();
            if (!workoutTypes.Any())
            {
                await _workoutLengthRepository.Add(new WorkoutLength { Name = "5-10 minutes" });
                await _workoutLengthRepository.Add(new WorkoutLength { Name = "10-15 minutes" });
                await _workoutLengthRepository.Add(new WorkoutLength { Name = "15-20 minutes" });
                await _workoutLengthRepository.Add(new WorkoutLength { Name = "20-25 minutes" });
                await _workoutLengthRepository.Add(new WorkoutLength { Name = "25-30 minutes" });

            }
        }

    }
    
 

}