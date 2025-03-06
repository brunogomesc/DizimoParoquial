using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DizimoParoquial.Controllers
{
    public class LaunchController : Controller
    {
        [SessionAuthorize("REGTIT")]
        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }
    }
}
