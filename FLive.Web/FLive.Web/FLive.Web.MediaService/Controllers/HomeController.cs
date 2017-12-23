using FLive.Web.MediaService.Helpers;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FLive.Web.MediaService.Controllers
{
    public class HomeController : Controller
    {
        
        [Authorize]
        [AdminModuleFilter]
        public ActionResult Index()
        {
            var model = Models.ViewModels.DashboardViewModel.Build();
            return View(model);
        }

        public ActionResult LogOut()
        {
            Request.GetOwinContext()
       .Authentication
       .SignOut(HttpContext.GetOwinContext()
                           .Authentication.GetAuthenticationTypes()
                           .Select(o => o.AuthenticationType).ToArray());
            return RedirectToAction("index", "Home");
        }
    }
}
