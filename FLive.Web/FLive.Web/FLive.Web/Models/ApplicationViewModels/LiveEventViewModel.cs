using System;

namespace FLive.Web.Models.ApplicationViewModels
{
    public class LiveEventViewModel
    {
        public string Title { get; set; }
        public long TrainingCategoryId { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public WorkoutLevel WorkoutLevel { get; set; }
        public long DurationMins { get; set; }
        public long PriceTierId { get; set; }
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
        //public WorkoutType WorkoutType { get; set; } = WorkoutType.Live;
        public int WorkoutType { get; set; }
        public bool IsPublishImmediately { get; set; }
        public DateTime PublishDateTime { get; set; }


    }

    public class CreateStreamViewModel
    {
        public string WowzaRegion { get; set; }
    }

    public class WorkoutCreateResult
    {
        public long WorkoutId { get; set; }
        public string ExternalAccessToken { get; set; }
    }
    

    public enum Location { India , Australia, USA}
    
}
