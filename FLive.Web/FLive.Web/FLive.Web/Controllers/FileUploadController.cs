using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLive.Web.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment hostingEnv;
        private readonly IFileUploadRepository _fileUploadRepository;

        public FileController(IHostingEnvironment env , IFileUploadRepository fileUploadRepository)
        {
            hostingEnv = env;
            _fileUploadRepository = fileUploadRepository;
        }

        #region commented code on 12 dec 2017

        //public IActionResult Index()
        //{
        //    return View();
        //}

       #endregion
        [HttpPost("upload")]
        [Consumes("multipart/form-data", "application/json-patch+json", "application/json")]

        public async Task<ActionResult> UploadFiles(IEnumerable<IFormFile> files)
        {
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                await _fileUploadRepository.UploadFileAsBlob(stream, file.FileName,"generic");
            }
            return RedirectToAction("Index");

        }

     
    }
}