namespace FLive.Web.Models
{
    public class UploadedFile
    {
        public string BaseUrl { get; set; }
        public string ContainerName { get; set; }
        public string OriginalFileName { get; set; }
        public string FileName { get; set; }
        public string Thumbnail { get; set; }
    }
}