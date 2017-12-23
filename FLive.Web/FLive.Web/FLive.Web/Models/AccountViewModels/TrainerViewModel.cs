using System.Collections.Generic;

namespace FLive.Web.Models.AccountViewModels
{
    public class TrainerViewModel
    {
        public TrainerViewModel()
        {
            UpcomingWorkouts = new List<LiveWorkout>();
            PastWorkouts = new List<LiveWorkout>();
        }
        public long[] TrainingGoals { get; set; }
        public long[] AreasOfSpcialisation { get; set; }
        public LevelOfCompetency LevelOfCompetency { get; set; }
        public string Bio { get; set; }
        public long LikeCount { get; set; }
        public long FollowerCount { get; set; }
        public string BSB { get; set; }
        public string AccountNumber { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCode { get; set; }
        public string Suburb { get; set; }
        public string Country { get; set; }


        public long VideoCount { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public LiveWorkout LatestWorkout { get; set; }
        public IEnumerable<LiveWorkout> UpcomingWorkouts { get; set; }
        public IEnumerable<LiveWorkout> PastWorkouts { get; set; }

        public bool IsFollowingTrainer { get; set; }
        public string StripeUserId { get; set; }
    }
}