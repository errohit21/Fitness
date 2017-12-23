using Newtonsoft.Json;

namespace FLive.Web.Models
{
    public class WorktoutBase : Entity
    {
        public string Title { get; set; }
        [JsonIgnore]
        public Trainer Trainer { get; set; }
        public long TrainerId { get; set; }
    }
}
