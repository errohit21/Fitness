using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Models.ApplicationViewModels;

namespace FLive.Web.Services
{
    public interface ILiveWorkoutService
    {
        Task<WorkoutCreateResult> Create(string getUserId, LiveEventViewModel liveEvent);
        Task SetPreviewVideo(long liveWorkoutId, string uploadedUrl);
        Task SetPreviewImage(long liveWorkoutId, string uploadedUrl);
        Task SetInstructionsFile(long liveWorkoutId, string uploadedUrl);
        Task<LiveWorkout> Get(long workoutId,long? subscriberId=null);
        Task<IEnumerable<LiveWorkout>> GetCurrentLiveWorkouts(long? subscriberId = null);
        Task<IEnumerable<LiveWorkout>> GetUpcomingVideosByUser(string userId);
        Task Subscribe(long liveWorkoutId, long subscriberId);
        Task<IEnumerable<Subscriber>> GetSubscribers(long liveWorkoutId);
        Task CompleteWorkout(long liveWorkoutId);
        Task<UpcomingWorkoutsContainer> GetUpcomingWorkouts(long? subscriberId=null);
        Task<IEnumerable<LiveWorkout>> GetTop(int i);
        Task<IEnumerable<LiveWorkout>> GetTopByCategory(int numberOfWorkouts, long categoryId);
        Task<UpcomingWorkoutsContainer> GetHomeDataForSignedInUser(string userId);
        Task<IEnumerable<LiveWorkout>> GetUpcomingWorkoutsByTrainer(long trainerId, long? subscriberId = null);
        Task<IEnumerable<LiveWorkout>> GetUpcomingVideosByCategory(long categoryId,long? subscriberId=null);
        Task<IEnumerable<LiveWorkout>> GetLiveVideosByCategory(long categoryId, long? subscriberId = null);

        Task LikeWorkout(long liveWorkoutId,long subscriberId);
        Task<IEnumerable<LiveWorkout>> Search(string searchText);
        Task Delete(long workoutId, string userId);
        Task UnSubscribe(long liveWorkoutId, long id);
		Task<LiveWorkout> GetLiveWorkoutByStreamId(string streamId);
		Task<UpcomingWorkoutsContainer> GetScheduledWorkouts(int pageId);
		Task<PaginatedResult<LiveWorkout>> GetAvailableWorkoutsPaginated(string userId,int pageId);
		Task<PaginatedResult<LiveWorkout>> GetAvailableWorkoutsByCategoryPaginated(string userId, long categoryId, int pageId);

		Task<PaginatedResult<LiveWorkout>> GetScheduledWorkoutsByCategoryPaginated(string userId, long categoryId, int pageId);
        Task<IEnumerable<LiveWorkout>> GetTrainersUnpublishedVideos(long trainerId);
    }
}