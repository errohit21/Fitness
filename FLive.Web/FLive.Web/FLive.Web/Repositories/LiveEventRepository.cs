using System.Collections.Generic;
using System.Linq;
using FLive.Web.Models;
using FLive.Web.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace FLive.Web.Repositories
{
    public class LiveEventRepository : ILiveEventRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LiveEventRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<LiveWorkout>> ListAll()
        {
            return await _dbContext.LiveWorkouts.ToListAsync();
        }

        public async Task Add(LiveWorkout liveEvent)
        {
            _dbContext.LiveWorkouts.Add(liveEvent);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<LiveWorkout> Get(long liveEventId)
        {
            return await _dbContext.LiveWorkouts.FirstOrDefaultAsync(a => a.Id == liveEventId);

        }
    }
}