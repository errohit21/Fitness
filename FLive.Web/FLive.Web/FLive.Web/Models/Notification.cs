using System;

namespace FLive.Web.Models
{
    public class Notification : Entity
    {
        public Notification()
        {
            EventDateTime = DateTime.UtcNow;
            NotificationStatus = NotificationStatus.Created;
        }
        public DateTime EventDateTime { get; set; }
        public  NotificationStatus NotificationStatus { get; set; }
        public string Title { get; set; }
        public  string Message { get; set; }

        public string UserId { get; set; }

        public string ProfileImage { get; set; }
        public string PreviewImagelUrl { get; set; }
        public string UserName { get; set; }
        public string MessageType { get; set; }
        public DateTime WorkoutStartTime { get; set; }
        public DateTime WorkoutEndTime { get; set; }
        public long? TrainerId { get; set; }
    }
}