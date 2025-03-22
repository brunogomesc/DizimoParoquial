using Microsoft.AspNetCore.Mvc;

namespace DizimoParoquial.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Error401()
        //{
        //    ViewBag.UserName = HttpContext.Session.GetString("Username");
        //    return View("/Views/Error/Index.cshtml");
        //}

    }
}
