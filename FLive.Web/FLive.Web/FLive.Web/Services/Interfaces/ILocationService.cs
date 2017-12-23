using System.Threading.Tasks;
using FLive.Web.Models.ApplicationViewModels;

namespace FLive.Web.Services.Interfaces
{
    public interface ILocationService
    {
        Task<int> GetPostCodeByLocation(LocationDataViewModel locationDataViewModel);
    }
}