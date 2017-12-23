using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FLive.Web.Models
{
    public class Subscriber : Entity
    {
        public Subscriber()
        {
            SubscriberTrainingGoals = new List<SubscriberTrainingGoal>();
            Workouts = new List<LiveWorkout>();
        }
        public ApplicationUser ApplicationUser { get; set; }
        public bool CreditCardVerified { get; set; }


        public IEnumerable<Trainer> Following { get; set; }
        public long FollowingCount { get; set; }

        public ICollection<LiveWorkout> Workouts { get; set; }
        public long WorkoutsCount { get; set; }

        public LevelOfCompetency LevelOfCompetency { get; set; }
        public ICollection<SubscriberTrainingGoal> SubscriberTrainingGoals { get; set; }

        public string StripeCustomerId { get; set; }
        public string Currency { get; set; }

        [NotMapped]
        public IEnumerable<LiveWorkout> FutureLiveWorkouts
        {
            get { return Workouts.Where(a => a.StartTime > DateTime.UtcNow).OrderBy(a=>a.StartTime); }
        }

        [NotMapped]
        public IEnumerable<LiveWorkout> PastLiveWorkouts
        {
            get { return Workouts.Where(a => a.StartTime < DateTime.UtcNow).OrderByDescending(a => a.StartTime); }
        }
    }
}