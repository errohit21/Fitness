using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;

namespace FLive.Web.Services.Interfaces
{
    public interface IAzurePushNotificationService
    {
      
        Task Register(ApplicationUser user);
		Task UnRegister(ApplicationUser applicationUser);

		Task SendLiveWorkoutCreatedNotificaiton(LiveWorkoutNotificationModel liveWorkoutModel);
        Task SendLiveWorkoutReminder(LiveWorkoutNotificationModel liveWorkoutModel);
        Task SendUserSubscribedNotificaiton(UserSubscribedNotificationModel notificationModel);
        Task<IEnumerable<Notification>> GetMyNotifications(string getUserId);
        Task SendUserLikedWorkoutNotificaiton(WorkoutLikedNotificationModel woroutLikedNotificationModel);
        Task PaymentReceivedNotificaiton(PaymentReceivedNotificationModel paymentReceivedNotificationModel);
        Task SendFollowNotification(long trainerId, long userId);
        Task SendWorkoutCancelledNotification(LiveWorkoutSubscriber subscriber, LiveWorkout workout);
        Task SendWorkoutCancelledNotification(Trainer trainer, LiveWorkout workout);
		Task SendLiveWorkoutCompletedNotificaiton(LiveWorkoutNotificationModel liveWorkoutModel);
        Task SendWorkoutPublishedNotification(LiveWorkoutNotificationModel liveWorkoutModel);
        Task<PaginatedResult<Notification>> GetMyNotificationsPaginated(string userId, int pageId);
    }
}