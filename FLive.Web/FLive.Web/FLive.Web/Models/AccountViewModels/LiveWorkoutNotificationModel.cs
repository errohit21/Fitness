namespace FLive.Web.Models.AccountViewModels
{
    public class LiveWorkoutNotificationModel
    {
        public long LiveWorkoutId { get; set; }

    }

    public class PaymentReceivedNotificationModel
    {
        public string UserId { get; set; }
        public string Amount { get; set; }
        public string WorkoutTitle { get; set; }
        public long LiveWorkoutId { get; set; }

    }



    public class UserSubscribedNotificationModel
    {
        public string WorkoutTitle { get; set; }
        public long LiveWorkoutId { get; set; }
        public string LiveWorkoutPreviewImageUrl { get; set; }

        public long SubscriberId { get; set; }
        public string SubscriberUserId { get; set; }
        public string TrainerUserId { get; set; }
        public string SubscriberName { get; set; }
        public string SubscriberProfileImage { get;set; }
        public string TrainerName { get; set; }

        public SubscriptionType SubscriptionType { get; set; }
    }

    public enum SubscriptionType { Subsribed, UnSubscribed};
    public class WorkoutLikedNotificationModel
    {
        public Subscriber Subscriber { get; set; }
        public LiveWorkout LiveWorkout { get; set; }
        public Trainer Trainer { get; set; }
    }
}