using System;

namespace FLive.Infrastructure.Models
{
    public class UpcomingLiveWorkout
    {
        public long Id { get; set; }
        public DateTime StartTime { get; set; }
		public DateTime CreatedTime { get; set; }

		public long LiveWorkoutId { get; set; }
        public string CreateStreamJson { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public long WorkoutType { get; set; }
        public bool IsPublishImmediately { get; set; }
        public long WorkoutPublishStatus { get; set; }
        public string MediaServiceUrl { get; set; }
    }
}