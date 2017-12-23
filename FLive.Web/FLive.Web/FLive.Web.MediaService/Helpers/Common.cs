using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLive.Web.MediaService.Helpers
{
    public static class Common
    {
        public static int? ParseInt(string val)
        {
            int convertuedValue;
            bool res = int.TryParse(val, out convertuedValue);
            if (res == false)
                return null;
            else
                return convertuedValue;
        }

        public static DateTime? ParseDate(string val)
        {
            DateTime convertuedDate;
            bool res = DateTime.TryParse(val, out convertuedDate);
            if (res == false)
                return null;
            else
                return convertuedDate;
        }
    }
}