namespace FLive.Infrastructure.Models
{
    public class SubscriberView
    {
        public long SubscriberId { get; set; }
        public string StripeCustomerId { get; set; }
        public string Currency { get; set; }
    }

    public class TrainerView
    {
        public long Id { get; set; }
        public string StripeUserId { get; set; }
     
    }
}