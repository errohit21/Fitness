using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using FLive.Web.MediaService.Models;

namespace FLive.Web.MediaService.Helpers
{
    public class AdminModuleFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var context = new ApplicationDbContext();
                if (!context.AdminUsers.Any(a => a.Username == filterContext.HttpContext.User.Identity.Name))
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Home",
                            action = "LogOut"
                        })
                    );
                }
            }
        }
    }
}