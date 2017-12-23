using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;

namespace FLive.Web.Services
{
    public interface IMasterDataService
    {
        Task<IEnumerable<TrainingCategory>> GetAllTrainingCategories();
        Task<string[]> GetAllAllowedTimezones();
        Task<IEnumerable<TrainingGoal>> GetAllTrainingGoals();
        Task<IEnumerable<PriceTier>> GetAllPriceTiers();
        Task<IEnumerable<Competency>> GetAllCompetencyLevels();
        Task<Settings> GetSettings();
        Task<IEnumerable<Country>> GetCountries();
	}
}