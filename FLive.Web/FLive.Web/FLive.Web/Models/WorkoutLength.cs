namespace FLive.Web.Models
{
	public class WorkoutLength : Entity
    {
        public int Lenght { get; set; }

        private string _name;
        public string Name
        {
            get { return $"{Lenght} mins"; }
            set { _name = value; }
        }


    }
}