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
    public class HeartbeatController : Controller
    {

        public HeartbeatController()
        {

        }

        [HttpGet]
        [Authorize]
        public string Get()
        {
            return $"Beats fine for {User.GetUserId()}";
        }

    }
}
