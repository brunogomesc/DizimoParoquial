using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class LoginController : Controller
    {

        private const string ROUTE_SCREEN_HOME = "/Views/Home/Index.cshtml";
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

            try
            {
                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
                {
                    _notification.AddErrorToastMessage("Usuário e/ou senha não preenchidos!");
                    return RedirectToAction(nameof(Index));
                }

                UserDTO userAuthenticated = await _userService.GetUserByUsernameAndPassword(user, password);

                if (userAuthenticated == null || userAuthenticated.UserId == 0)
                {
                    _notification.AddErrorToastMessage("Usuário não é válido ou está inativo!");
                    return RedirectToAction(nameof(Index));
                }

                HttpContext.Session.SetInt32("User", userAuthenticated.UserId);
                HttpContext.Session.SetString("Username", userAuthenticated.Username);

                ViewBag.UserName = userAuthenticated.Username;

                _notification.AddSuccessToastMessage("Usuário autenticado com sucesso!");

                return View(ROUTE_SCREEN_HOME);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

    }
}
