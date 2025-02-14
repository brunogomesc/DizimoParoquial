using System.Diagnostics;
using DizimoParoquial.Models;
using Microsoft.AspNetCore.Mvc;

namespace DizimoParoquial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(User userAuthenticated)
        {
            return View(userAuthenticated);
        }
    }
}
