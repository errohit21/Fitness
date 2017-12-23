using FLive.Media.Common.Contracts;
using FLive.Media.Common.Implementations;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLive.Web.MediaService.Controllers
{
    public class BaseMediaController : Controller
    {
        private IMediaService _mediaService;
        private IAssetService _assetSErvice;
        private IJobService _jobService;
        private ILocatorService _locatorService;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            if (this._mediaService == null)
            {
                this._mediaService = new Media.Common.Implementations.MediaService(new CloudMediaContext(ConfigurationManager.AppSettings["MediaServerAccountName"],
                    ConfigurationManager.AppSettings["MediaServerAccountKey"]));
                this._assetSErvice = new AssetService(this.MediaService);
                this._jobService = new JobService(this.MediaService);
                this._locatorService = new LocatorService(this.MediaService);
            }
            base.Initialize(requestContext);
        }

        protected IMediaService MediaService
        {
            get
            {
                return this._mediaService;
            }
        }

        protected IAssetService AssetService
        {
            get { return this._assetSErvice; }
        }

        protected IJobService JobService
        {
            get
            {
                return this._jobService;
            }
        }

        protected ILocatorService LocatorService
        {
            get { return this._locatorService; }
        }

    }
}

