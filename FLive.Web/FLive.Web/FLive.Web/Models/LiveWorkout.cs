using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace FLive.Web.Models
{
    public class LiveWorkout  :Entity
    {
        public LiveWorkout()
        {
            LiveWorkoutSubscribers = new List<LiveWorkoutSubscriber>();
            LiveWorkoutFavors = new List<LiveWorkoutFavor>();
        }

      

        //public Trainer Trainer { get; set; }
        public string Title { get; set; }
        [JsonIgnore]
        public Trainer Trainer { get;set; }
        public long TrainerId { get; set; }
        [JsonIgnore]
        public TrainingCategory TrainingCategory{get;set;}
        public long TrainingCategoryId{get;set;}
        public DateTime CreatedTime { get; set; }
        public DateTime StartTime { get; set; }
        public long DurationMins { get; set; }
        public DateTime EndTime { get; set; }
        public string StreamId { get; set; }
        public string RecordingUrl { get; set; }
        public string PreviewVideoUrl { get; set; }
        public string PreviewImagelUrl { get; set; }
        public PriceTier PriceTier { get; set; }
        public long PriceTierId { get; set; }

       
        public Transaction Transaction { get; set; }
        public long TransactionId { get; set; }

        public WorkoutLevel WorkoutLevel { get; set; }
        public long SubscriberCount { get; set; }
        public long LikeCount { get; set; }

        public WorkoutStatus WorkoutStatus { get; set; }
        public WorkoutPublishStatus WorkoutPublishStatus { get; set; }

        [JsonIgnore]
        public ICollection<LiveWorkoutSubscriber> LiveWorkoutSubscribers { get; set; }
        [JsonIgnore]
        public List<LiveWorkoutFavor> LiveWorkoutFavors { get; set; }
      

        public bool IsAlreadySubsribed(long? subsriberId)
        {
            return subsriberId.HasValue && LiveWorkoutSubscribers.Any(a => a.SubscriberId == subsriberId);
        }

        public bool IsAlreadyLiked(long? subsriberId)
        {
            return subsriberId.HasValue && LiveWorkoutFavors.Any(a => a.SubscriberId == subsriberId);
        }


        public bool IsAleadyFollowingTrainer(long? subsriberId)
        {
            return subsriberId.HasValue && Trainer.TrainerSubscribers.Any(a => a.SubscriberId == subsriberId);
        }

        [NotMapped]
        public bool IsSubscribed { get; set; }
        [NotMapped]
        public bool IsFollowingTrainer { get; set; }

        [NotMapped]
        public string TrainerName { get; set; }
        [NotMapped]
        public string TrainerProfileImageUrl { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }

        public bool IsArchived { get;  set; }

        //public WorkoutType WorkoutType { get; set; }
        public int WorkoutType { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsPublishImmediately { get; set; }

        public DateTime? PublishDateTime { get; set; }

        public long? SponsorId { get; set; }
        public Sponsor Sponsor { get; set; }

        public string InstructionsLink { get; set; }

        public string ExternalAccessToken { get; set; }
        
    }

    //not plugged to the context yet
}
