using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;

namespace FLive.Web.Repositories.Interfaces
{
    public interface IStreamRepository
    {
        Task<IEnumerable<Stream>> ListAll();
        Task Add(Stream stream);
    }
}