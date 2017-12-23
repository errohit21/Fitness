namespace FLive.Web.Models
{
    public class TrainerFollower : Entity
    {
        public Trainer Trainer { get; set; }
        public long TrainerId { get; set; }
        public Subscriber Subscriber { get; set; }
        public long SubscriberId { get; set; }
    }

    public class TrainerSubsriber : Entity
    {
        public Trainer Trainer { get; set; }
        public long TrainerId { get; set; }
        public Subscriber Subscriber { get; set; }
        public long SubscriberId { get; set; }
    }


}