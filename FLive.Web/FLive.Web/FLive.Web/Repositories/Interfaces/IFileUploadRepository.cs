using System.IO;
using System.Threading.Tasks;


namespace FLive.Web.Repositories
{
    public interface IFileUploadRepository
    {
        Task<string> UploadFileAsBlob(Stream stream, string filename, string containerName);
    }
}