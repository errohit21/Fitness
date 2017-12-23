using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;

namespace FLive.Web.Models
{
    public class Trainer :Entity
    {
        public Trainer()
        {
            TrainerTrainingCategories =  new List<TrainerTrainingCategory>();
            TrainerTrainingGoals =  new List<TrainerTrainingGoal>();
            TrainerSubscribers = new List<TrainerSubsriber>();
            TrainerFollowers = new List<TrainerFollower>();
            LiveWorkouts =  new List<LiveWorkout>();
            TrainerFavors = new List<TrainerFavor>();
        }
        public ApplicationUser ApplicationUser { get; set; }
        
        public string Bio { get; set; }
        public LevelOfCompetency LevelOfCompetency { get; set; }

        public string BSB { get; set; }
        public string AccountNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCode { get; set; }
        public string Suburb { get; set; }
        public string Country { get; set; }

        public bool IsVerified { get; set; }
        public long LikeCount { get; set; }

        [JsonIgnore]
        public string SripeAuthResult { get;set; }
        public string StripeUserId { get; set; }

        public ICollection<TrainerTrainingCategory> TrainerTrainingCategories { get; set; }
        public ICollection<TrainerTrainingGoal> TrainerTrainingGoals { get; set; }
        public ICollection<LiveWorkout> LiveWorkouts { get; set; }

        [JsonIgnore]
        public ICollection<TrainerSubsriber> TrainerSubscribers { get; set; }
        public ICollection<TrainerFollower> TrainerFollowers { get; set; }

        public ICollection<TrainerFavor> TrainerFavors { get; set; }

        public bool IsDeleted { get; set; }


        [NotMapped]
        public IEnumerable<LiveWorkout> PastLiveWorkouts
        {
            get { return LiveWorkouts.Where(a => a.StartTime < DateTime.UtcNow).OrderByDescending(a=>a.StartTime); }
        }

        [NotMapped]
        public LiveWorkout CurrentLiveWorkout
        {
            get;set;
        }

        [NotMapped]
        public IEnumerable<LiveWorkout> FutureLiveWorkouts
        {
            get { return LiveWorkouts.Where(a => a.StartTime >  DateTime.UtcNow); }
        }

        public bool IsAleadyFollowingTrainer(long? subsriberId)
        {
            return subsriberId.HasValue && TrainerSubscribers.Any(a => a.SubscriberId == subsriberId);
        }


        [NotMapped]
        public bool IsSubscribed { get; set; }

        public bool IsAlreadySubsribed(long? subsriberId)
        {
            return subsriberId.HasValue && TrainerSubscribers.Any(a => a.SubscriberId == subsriberId);
        }

    }

    public enum LevelOfCompetency
    {
        Unspecified=0,
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
}