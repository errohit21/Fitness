using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Data;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Repositories;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FLive.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly IAzurePushNotificationService _azurePushNotifications;

        public NotificationsController(IAzurePushNotificationService azurePushNotifications)
        {
            _azurePushNotifications = azurePushNotifications;
        }


        [HttpPost("liveworkoutcreated")]
        public async Task LiveWorkoutCreated([FromBody]LiveWorkoutNotificationModel liveWorkoutModel)
        {
            await _azurePushNotifications.SendLiveWorkoutCreatedNotificaiton(liveWorkoutModel);
        }

        [HttpPost("paymentreceived")]
        public async Task PaymentReceived([FromBody]PaymentReceivedNotificationModel paymentReceivedNotificationModel)
        {
            await _azurePushNotifications.PaymentReceivedNotificaiton(paymentReceivedNotificationModel);
        }


        [HttpPost("liveworkoutreminder")]
        public async Task LiveWorkoutReminder([FromBody]LiveWorkoutNotificationModel liveWorkoutModel)
        {
            await _azurePushNotifications.SendLiveWorkoutReminder(liveWorkoutModel);
        }

        [HttpPost("workoutpublished")]
        public async Task WorkoutPublishedNotification([FromBody]LiveWorkoutNotificationModel liveWorkoutModel)
        {
            await _azurePushNotifications.SendWorkoutPublishedNotification(liveWorkoutModel);
        }



        [HttpGet("mynotifications")]
        public async Task<IEnumerable<Notification>> MyNotifications()
        {
          return  await _azurePushNotifications.GetMyNotifications(User.GetUserId());
        }

        [HttpGet("mynotifications/{pageId}")]
        public async Task<PaginatedResult<Notification>> MyNotifications(int pageId = 1)
        {
            return await _azurePushNotifications.GetMyNotificationsPaginated(User.GetUserId() , pageId);
        }

    }
}
