using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLive.Web.MediaService.Models
{
    [Serializable]
    public class EntityBase
    {
        public int Id { get; set; }
        public bool Archived { get; set; }
        public DateTime? DateLastUpdated { get; set; }
        public string UserLastUpdated { get; set; }
        public DateTime? DateCreated { get; set; }
        public string UserCreated { get; set; }
    }
}