using FLive.Web.MediaService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FLive.Web.MediaService.Controllers.API
{
    public class MediaAssetController : ApiController
    {
        [HttpGet]
        public MediaAsset GetMediaAsset(int id)
        {
            var context = new ApplicationDbContext();
            return context.MediaAssets.FirstOrDefault(a => a.Id == id);
        }
    }
}
