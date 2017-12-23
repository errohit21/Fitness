using System.Threading.Tasks;
using FLive.Web.Models;

namespace FLive.Web.Services
{
    public interface INotificationsService
    {
        Task SendNewSubscriptionNotificaiton(PushNotification newSubscriptionNotification);
        Task Register(PushNotification newSubscriptionNotification);
    }
}