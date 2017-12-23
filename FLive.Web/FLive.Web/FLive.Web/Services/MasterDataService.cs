using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Repositories;
using Microsoft.CodeAnalysis.CSharp;

namespace FLive.Web.Services
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IGenericRepository<TrainingCategory> _trainingCategoryRepository;
        private readonly IGenericRepository<TrainingGoal> _trainingGoalRepository;
        private readonly IGenericRepository<PriceTier> _priceTierRepository;
        private readonly IGenericRepository<Competency> _competencyRepository;
        private readonly IGenericRepository<Settings> _settingsRepository;
        private readonly IGenericRepository<Country> _countryRepository;
        private readonly IGenericRepository<WorkoutType> _workoutTypeRepository;
        private readonly IGenericRepository<WorkoutLength> _workoutLengthRepository;

        public MasterDataService(IGenericRepository<TrainingCategory> trainingCategoryRepository , 
            IGenericRepository<TrainingGoal> trainingGoalRepository , 
            IGenericRepository<PriceTier> priceTierRepository,
            IGenericRepository<Competency> competencyRepository , 
			IGenericRepository<Settings> settingsRepository ,
            IGenericRepository<Country> countryRepository,
            IGenericRepository<WorkoutType> workoutTypeRepository,
            IGenericRepository<WorkoutLength> workoutLengthRepository
         )
        {
            _trainingCategoryRepository = trainingCategoryRepository;
            _trainingGoalRepository = trainingGoalRepository;
            _priceTierRepository = priceTierRepository;
            _competencyRepository = competencyRepository;
            _settingsRepository = settingsRepository;
			_countryRepository = countryRepository;
            _workoutTypeRepository = workoutTypeRepository;
            _workoutLengthRepository = workoutLengthRepository;
        }

        public async Task<IEnumerable<TrainingCategory>> GetAllTrainingCategories()
        {
            return await _trainingCategoryRepository.GetAll();
        }
        public async Task<IEnumerable<TrainingGoal>> GetAllTrainingGoals()
        {
            return await _trainingGoalRepository.GetAll();
        }

        public async Task<IEnumerable<PriceTier>> GetAllPriceTiers()
        {
           return await _priceTierRepository.GetAll();
        }

        public async Task<IEnumerable<Competency>> GetAllCompetencyLevels()
        {
            return await _competencyRepository.GetAll();
        }

        public async Task<string[]> GetAllAllowedTimezones()
        {
            return await Task.FromResult(GetAllAustralianTimezones());
        }

        public async Task<Settings> GetSettings()
        {
            return await _settingsRepository.GetFirstOrDefault(a => a.Id > 0);
        }

        private string[] GetAllAustralianTimezones()
        {
            return new[] {"E. Australia Standard Time", "Cen. Australia Standard Time", "W. Australia Standard Time"};
        }

		public async Task<IEnumerable<Country>> GetCountries()
		{
			return await _countryRepository.GetAll();
		}
        public async Task<IEnumerable<WorkoutType>> GetWorkoutType()
        {
            return await _workoutTypeRepository.GetAll();
        }
        public async Task<IEnumerable<WorkoutLength>> GetWorkoutLength()
        {
            return await _workoutLengthRepository.GetAll();
        }
    }
}