using Microsoft.WindowsAzure.MediaServices.Client;

namespace FLive.Media.Common.Models
{
    public class Asset
    {
        public IAsset MediaAsset { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
