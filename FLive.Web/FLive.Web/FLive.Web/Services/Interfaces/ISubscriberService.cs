using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;

namespace FLive.Web.Services
{
    public interface ISubscriberService
    {
        Task CreateSubscriber(ApplicationUser applicationUser, string deviceToken);
        Task<Subscriber> GetByUserId(string userId);
        Task<IEnumerable<Subscriber>> GetAll();
        Task<Subscriber> GetSubscriberByAspnetUserId(string userId);
        Task<Subscriber> Get(long subscriberId);
        Task UpdateSubscriber(ProfileViewModel profilePostViewModel);
        Task<SubscriberModal> GetSubscriberProfile(string id);
    }
}