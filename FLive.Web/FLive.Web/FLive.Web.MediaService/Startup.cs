using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FLive.Web.MediaService.Startup))]
namespace FLive.Web.MediaService
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
