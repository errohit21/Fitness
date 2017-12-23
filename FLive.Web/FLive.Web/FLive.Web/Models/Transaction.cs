using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FLive.Web.Models
{
    public class Transaction : Entity
    {
        public long TrainerId { get; set; }

        [JsonIgnore]
        public LiveWorkout LiveWorkout { get; set; }
        [NotMapped]
        public string WorkoutTitle { get { return LiveWorkout.Title; } }

        [NotMapped]
        public DateTime StartTime { get { return LiveWorkout.StartTime; } }


        [NotMapped]
        public long SubscriberCount { get { return LiveWorkout.SubscriberCount; } }

        [NotMapped]
        public decimal TotalEarning { get { return Amount; } }

        [NotMapped]
        public string IapKey { get { return LiveWorkout.PriceTier.IapKey; } }
        public long LiveWorkoutId { get; set; }

        public long SubscriberId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TranactionDateTime { get; set; }
        public bool IsSuccessfull { get; set; }
        public string Notes { get; set; }
    }


}