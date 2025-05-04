using System.Diagnostics;
using DizimoParoquial.Models;
using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    //[SessionAuthorize("HOMEPAGE")]
    public class HomeController : Controller
    {
        private readonly IToastNotification _notification;

        public HomeController(IToastNotification notification)
        {
            _notification = notification;
        }

        public IActionResult Index()
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            return View();
        }

        public IActionResult HomeAgents()
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            return View();
        }
    }
}
