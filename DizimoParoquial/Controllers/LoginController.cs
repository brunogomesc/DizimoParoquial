using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class LoginController : Controller
    {

        private readonly IToastNotification _notification;

        public LoginController(IToastNotification notification)
        {
            _notification = notification;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string user, string password)
        {

            if(string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                _notification.AddErrorToastMessage("Usuário e/ou senha não preenchidos!");
                return RedirectToAction("Index", "Login");
            }
                
            return RedirectToAction("Index", "Home", new { typeUser = "admin"});
        } 

    }
}
