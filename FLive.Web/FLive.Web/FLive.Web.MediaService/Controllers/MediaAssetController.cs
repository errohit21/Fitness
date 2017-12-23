using FLive.Web.MediaService.Helpers;
using FLive.Web.MediaService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLive.Web.MediaService.Controllers
{
    [AdminModuleFilter]
    public class MediaAssetController : Controller
    {
        // GET: MediaAsset
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            return View(context.MediaAssets.ToList().OrderByDescending(a=> a.Id).ToList());
        }

        public ActionResult Details(int assetId)
        {
            var context = new ApplicationDbContext();
            return View(context.MediaAssets.Single(a=> a.Id == assetId));
        }
    }
}