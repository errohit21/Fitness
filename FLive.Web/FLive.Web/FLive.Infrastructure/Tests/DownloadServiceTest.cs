using System.Configuration;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FLive.Infrastructure.Tests
{
    [TestFixture]
    public class DownloadServiceTest
    {
        private readonly string _dbConnection;
        private readonly string _apiEndpoint;
        private readonly string _apiToken;


        public DownloadServiceTest()
        {
            _dbConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            _apiEndpoint = ConfigurationManager.AppSettings["apiEndpoint"];
            _apiToken = ConfigurationManager.AppSettings["apiToken"];
        }

        [Test]
        public async Task create_should_return_a_valid_stream_id()
        {
            var downloadService = new DownloadService(_dbConnection );
            await downloadService.DownloadRecordings();

        }

    }
}
