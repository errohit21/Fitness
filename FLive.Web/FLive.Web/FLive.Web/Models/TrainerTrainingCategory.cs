namespace FLive.Web.Models
{
    public class TrainerTrainingCategory : Entity
    {
        public long TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public long TrainingCategoryId { get; set; }
        public TrainingCategory TrainingCategory { get; set; }

    }
}