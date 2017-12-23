using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FLive.Web.Models.ApplicationViewModels;
using FLive.Web.Models;

namespace FLive.Web.Services
{
    public interface IStreamManagementService
    {
        Task<string> Create(CreateStreamViewModel createStreamViewModel);
        Task<string> Get(string streamId);
        Task<string> Start(string streamId);
        Task<string> Stop(string streamId);
        Task<string> Delete(string streamId);
        Task<string> GetAll();


    }
}
