using System.Collections.Generic;
using FLive.Web.Models;
using System.Threading.Tasks;

namespace FLive.Web.Repositories
{
    public interface ILiveEventRepository
    {
        Task<IEnumerable<LiveWorkout>> ListAll();
        Task Add(LiveWorkout liveEvent);

        Task<LiveWorkout> Get(long eventId);

    }
}