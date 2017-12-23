using System.Collections.Generic;

namespace FLive.Web.Models.AccountViewModels
{
    public class SubscriberModal
    {
        public SubscriberModal()
        {
            UpcomingWorkouts = new List<LiveWorkout>();
            PastWorkouts = new List<LiveWorkout>();
        }
        public long[] TrainingGoals { get; set; }
        public LevelOfCompetency LevelOfCompetency { get; set; }
        public long WorkoutsCount { get; set; }
        public long FollowingCount { get; set; }
        public string StripeCustomerId { get; set; }
        public long Id { get; set; }
        public IEnumerable<LiveWorkout> UpcomingWorkouts { get; set; }
        public IEnumerable<LiveWorkout> PastWorkouts { get; set; }
        public LiveWorkout LatestWorkout { get; set; }


    }
}
