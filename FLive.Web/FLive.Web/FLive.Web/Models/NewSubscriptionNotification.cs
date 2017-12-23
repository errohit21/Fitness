namespace FLive.Web.Models
{
    public class PushNotification : Notification
    {
        public DeviceType DeviceType { get; set; }
        public string Tags { get; set; }
        public string DeviceToken { get; set; }
    }
}