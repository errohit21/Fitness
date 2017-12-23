namespace FLive.Web.Models
{
    public class TrainingCategory : Entity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        private string _description;
        public string Description
        {
            get { return $"{LiveWorkoutCount} Live Workouts"; }
            set { _description = value; } }
        public string DescriptionColor { get; set; }
        public string NameColor { get; set; }
        public long LiveWorkoutCount { get; set; }

        public bool IsSponsorCategory { get; set; }
        public string SponsorLogo { get; set; }
        public string SponsorText { get; set; }
    }

}