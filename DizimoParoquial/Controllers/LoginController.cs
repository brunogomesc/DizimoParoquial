using Microsoft.AspNetCore.Mvc;

namespace DizimoParoquial.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string user, string password)
        {
            return RedirectToAction("Index", "Home", new { typeUser = "admin"});
        } 

    }
}
