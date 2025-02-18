using Microsoft.AspNetCore.Mvc;

namespace DizimoParoquial.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
