using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Net.Http.Headers;

namespace FLive.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult SignalR()
        {
            return View();
        }
        public IActionResult WellKnownAppleHandoff()
        {
            //return Json(new { foo = "bar", baz = "Blech" });

            var stringContent = "{\n \"applinks\": {\n        \"apps\": [],\n        \"details\": [\n            {\n                \"appID\": \"9C9LQ65Z8T.au.com.flive\",\n                \"paths\": [ \"/Workout/*\"]\n            }\n        ]\n    }\n}";
            //return new JsonResult("{\n \"applinks\": {\n        \"apps\": [],\n        \"details\": [\n            {\n                \"appID\": \"9C9LQ65Z8T.au.com.flive\",\n                \"paths\": [ \"/workout/*\"]\n            }\n        ]\n    }\n}");

            Response.ContentType = "application/octet-stream";

            return this.Content(stringContent);
            
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
