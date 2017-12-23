using System.Collections.Generic;

namespace FLive.Web.Models
{
    public class PaginatedResult<T>
    {
       public IEnumerable<T> Results  { get; set; }
        public long PageCount { get; set; }
        public long Total { get; set; }
    }


}