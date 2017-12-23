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
    public class TrainerController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage UpdateActiveStatus(ActiveStatusUpdateViewModel model)
        {
            var status = Trainer.UpdateActiveStatus(model);
            if (status.IsSuccessfull)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
