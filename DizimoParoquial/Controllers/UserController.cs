using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class UserController : Controller
    {

        private readonly IToastNotification _notification;
        private readonly UserService _userService;

        public UserController(IToastNotification notification, UserService userService)
        {
            _notification = notification;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(User user)
        {

            if (!ModelState.IsValid)
            {
                _notification.AddErrorToastMessage("Dados de cadastro não estão válidos!");
                return RedirectToAction(nameof(Index));
            }

            bool userWasCreated = await _userService.RegisterUser(user);

            if (userWasCreated)
                _notification.AddSuccessToastMessage("Usuário criado com sucesso!");
            else
                _notification.AddErrorToastMessage("Não foi possível criar o usuário!");

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult EditUser()
        {
            return View();
        }
    }
}
