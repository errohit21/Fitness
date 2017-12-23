using System.Collections.Generic;

namespace FLive.Web.Models.AccountViewModels
{
    public class UpcomingWorkoutsContainer
    {
        public UpcomingWorkoutsContainer()
        {
            WorkoutCategoryGroups = new List<WorkoutCategoryContainer>();
            Cards = new List<LiveWorkout>();
        }
        public IList<WorkoutCategoryContainer> WorkoutCategoryGroups { get; set; } 
		public WorkoutCategoryContainer AllAvailableWorkoutsContainer { get; set; }
        public ICollection<LiveWorkout> Cards { get; set; } 
    }

    public class WorkoutCategoryContainer
    {
        
        public string CategoryName { get; set; }
        public ICollection<LiveWorkout> LiveWorkouts { get; set; }
    }

    public class SearchResultsContainer
    {

        public IEnumerable<LiveWorkout> LiveWorkouts { get; set; }

        public IEnumerable<TrainerSearchResultModel> Trainers { get; set; }
    }
}