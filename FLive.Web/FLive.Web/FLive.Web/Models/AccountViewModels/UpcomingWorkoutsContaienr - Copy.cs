using System.Collections.Generic;

namespace FLive.Web.Models.AccountViewModels
{
    public class UpcomingWorkoutsContaienr1
    {
        public UpcomingWorkoutsContaienr1()
        {
            WorkoutCategoryGroups1 = new List<WorkoutCategoryContainer1>();
        }
        public ICollection<WorkoutCategoryContainer1> WorkoutCategoryGroups1 { get; set; } 
    }

    public class WorkoutCategoryContainer1
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<LiveWorkout> FutureWorkouts { get; set; }
        public ICollection<LiveWorkout> LiveWorkouts { get; set; }
    }
}