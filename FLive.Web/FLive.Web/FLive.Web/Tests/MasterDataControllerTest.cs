using Xunit;
using FLive.Web.Controllers;
using FLive.Web.Controllers.API;
using FLive.Web.Data;
using Microsoft.AspNetCore.Hosting.Internal;

namespace FLive.Web.Tests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class MasterDataControllerTest
    {

        private ApplicationDbContext _applicationDbContext;
        private MasterDataController _repository;
        private void Initialize()
        {
            //var db = new ApplicationDbContext();
            //db.User
            //_context = new HostingApplication.Context(db.Options);
            //_repository = new BrandsRepository(_context);

            //_context.Database.EnsureDeleted();
            //_context.Database.EnsureCreated();
        }


        [Fact]
        public void add_category()
        {
            Assert.Equal(4, 4);
        }

    }
}
