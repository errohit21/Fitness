using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Models.ApplicationViewModels;

namespace FLive.Web.Services
{
    public interface ITrainerService
    {
        Task<Trainer> Get(long trainerId, long? subscriberId = null);
        Task<IEnumerable<Trainer>> GetSubscribedTrainers(long subscriberId);
        Task<Trainer> GetWithSubscribers(long userId);
        Task<IEnumerable<Trainer>> GetAll();
        Task<Trainer> GetByUserId(string userId);
        Task<IEnumerable<Trainer>> GetTrendingTrainers();
        Task Follow(long trainerId, long setterId);
        Task CreateTrainer(ApplicationUser applicationUse , string deviceToken);
        Task<Trainer> GetTrainerByAspnetUserId(string userId);
        Task UpdateTrainer(ProfileViewModel trainer);
        Task<TrainerViewModel> GetTrainerProfile(string userId, long? loggedInUserId = null);
        Task<IEnumerable<TrainerSearchResultModel>> Search(string searchText);
        Task<IEnumerable<Trainer>> GetTrainersFromMyLocationByDistance(LocationDataViewModel postCode);
        Task UpdateStripeToken(long trainerId, string stripeToken,string stripeAuthResult);
        Task Subscribe(long trainerId, long id);
        Task UnSubscribe(long trainerId, long id);
    }
}