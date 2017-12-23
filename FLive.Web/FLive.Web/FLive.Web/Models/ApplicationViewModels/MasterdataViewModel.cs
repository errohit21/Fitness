using System.Collections.Generic;

namespace FLive.Web.Models.ApplicationViewModels
{
    public class MasterdataViewModel
    {
        public TrainerMasterData Trainer { get; set; }
        public SubscriberMasterData User { get; set; }

        public Settings Settings { get; set; }

		public IEnumerable<Country> Countries { get; set; }
       
    }

    public class SubscriberMasterData
    {
        public IEnumerable<Competency> Competencies { get; set; }
        public IEnumerable<TrainingGoal> TrainingGoals { get; set; }
        public string Instructions { get; set; }

    }

    public class TrainerMasterData
    {
        public IEnumerable<TrainingCategory> TrainingCategories { get; set; }
        public IEnumerable<Competency> Competencies { get; set; }
        public IEnumerable<TrainingGoal> TrainingGoals { get; set; }
        public IEnumerable<PriceTier> PriceTiers { get; set; }
        public IEnumerable<WorkoutLength> WorkoutLengths { get; set; }
        public string Instructions { get; set; }
    }
}