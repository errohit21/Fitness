using System;

namespace FLive.Web.Models
{
    public class UpcomingLiveWorkout : Entity
    {
        public DateTime StartTime { get; set; }
        public long LiveWorkoutId { get; set; }
        public string CreateStreamJson { get; set; }
        public bool IsPublishImmediately { get; set; }
        public DateTime? PublishDateTime { get; set; }
        //public WorkoutType WorkoutType { get; set; } = WorkoutType.Live;
        public int WorkoutType { get; set; }


        public WorkoutPublishStatus WorkoutPublishStatus { get; set; }
        public string MediaServiceUrl { get; set; }

    }
}