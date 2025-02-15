using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class LoginController : Controller
    {

        private readonly IToastNotification _notification;
        private readonly UserService _userService;

        public LoginController(IToastNotification notification, UserService userService)
        {
            _notification = notification;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string user, string password)
        {

            if(string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                _notification.AddErrorToastMessage("Usuário e/ou senha não preenchidos!");
                return RedirectToAction(nameof(Index));
            }

            User userAuthenticated = await _userService.GetUserByUsernameAndPassword(user, password);

            if (userAuthenticated.UserId == 0)
            {
                _notification.AddErrorToastMessage("Não retornou usuário!");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notification.AddErrorToastMessage("retornou usuário!");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index", "Home", userAuthenticated);
        } 

    }
}
