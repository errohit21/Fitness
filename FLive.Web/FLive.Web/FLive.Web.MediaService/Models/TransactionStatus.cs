using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLive.Web.MediaService.Models
{
    public class TransactionStatus
    {
        public bool IsSuccessfull { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}