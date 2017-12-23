using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Repositories;
using FLive.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLive.Web.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class SubsriberController : Controller
    {
        private readonly ISubscriberService _subscriberService;
        private readonly IFileUploadRepository _fileUploadRepository;
        public SubsriberController(ISubscriberService subscriberService , IFileUploadRepository fileUploadRepository)
        {
            _subscriberService = subscriberService;
            _fileUploadRepository = fileUploadRepository;
        }


        [HttpGet("all")]
        public async Task<IEnumerable<Subscriber>> GetAll(string trainerId)
        {
            throw new NotImplementedException();
          //  return await _subscriberService.GetAll();
        }

        


    }
}