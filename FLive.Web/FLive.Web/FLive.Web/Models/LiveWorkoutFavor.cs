namespace FLive.Web.Models
{
    public class LiveWorkoutFavor : Entity
    {
        public long LiveWorkoutId { get; set; }
        public LiveWorkout LiveWorkout { get; set; }
        public Subscriber Subscriber { get; set; }
        public long SubscriberId { get; set; }
    }
}