using Microsoft.AspNetCore.SignalR;

namespace FLive.Web.SignalR
{

    public class WorkoutNotificationsHub : Hub
    {
        public void JoinGroup(string groupName)
        {
            this.Groups.Add(this.Context.ConnectionId, groupName);
        }
    }
}
