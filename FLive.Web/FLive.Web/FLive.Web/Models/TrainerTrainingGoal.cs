namespace FLive.Web.Models
{
    public class TrainerTrainingGoal : Entity
    {
        public long TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public long TrainingGoalId { get; set; }
        public TrainingGoal TrainingGoal { get; set; }

    }

    public class SubscriberTrainingGoal : Entity
    {
        public long SubscriberId { get; set; }
        public Subscriber Subscriber { get; set; }
        public long TrainingGoalId { get; set; }
        public TrainingGoal TrainingGoal { get; set; }

    }
}