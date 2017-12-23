using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLive.Web.MediaService.Models
{
    public class MediaAsset : EntityBase
    {
        public string OriginalFileName { get; set; }
        public string Status { get; set; }
        public Double CurrentProgress { get; set; }
        public string Mesage { get; set; }
        public DateTime? DateUploadStarted { get; set; }
        public DateTime? DateUploadCompleted { get; set; }
        public DateTime? DateSyncToServiceStarted { get; set; }
        public DateTime? DateSyncToServiceCompleted { get; set; }
        public DateTime? DateEncodingStarted { get; set; }
        public DateTime? DateEncodingCompleted { get; set; }
        public DateTime? DatePublished { get; set; }
        public DateTime? DateExpire { get; set; }
        public string MediaServiceOriginalAssetID { get; set; }
        public string MediaServiceEncodedAssetID { get; set; }

        public bool IsUploadCompleted { get; set; }
        public bool IsSyncCompleted { get; set; }
        public bool IsEncodingCompleted { get; set; }

        public string DashUrl { get; set; }
        public string HSLUrl { get; set; }
        public string StreamingUrl { get; set; }

        public string FliveWorkoutId { get; set; }
    }
}