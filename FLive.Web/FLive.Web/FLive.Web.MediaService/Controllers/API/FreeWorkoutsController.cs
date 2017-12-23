using FLive.Web.MediaService.Models.flive;
using FLive.Web.MediaService.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FLive.Web.MediaService.Controllers.API
{
    public class FreeWorkoutsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage UpdateWorkoutFreeStatus(ActiveStatusUpdateViewModel model)
        {
            var status = LiveWorkout.UpdateWorkoutFreeStatus(model);
            if (status.IsSuccessfull)
            {
                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
