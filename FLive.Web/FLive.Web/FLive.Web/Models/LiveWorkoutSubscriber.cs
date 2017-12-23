namespace FLive.Web.Models
{
    public class LiveWorkoutSubscriber : Entity
    {
        public long LiveWorkoutId { get; set; }
        public LiveWorkout LiveWorkout { get; set; }

        public long SubscriberId { get; set; }
        public Subscriber Subscriber { get; set; }
    }
}