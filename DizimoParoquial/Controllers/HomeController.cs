using System.Diagnostics;
using DizimoParoquial.Models;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class HomeController : Controller
    {
        private readonly IToastNotification _notification;

        public HomeController(IToastNotification notification)
        {
            _notification = notification;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
