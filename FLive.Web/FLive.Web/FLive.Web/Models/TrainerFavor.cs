namespace FLive.Web.Models
{
    public class TrainerFavor  :Entity
    {
        public long TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public ApplicationUser Favorer { get; set; }
        public long FavorerId { get; set; }
    }
}