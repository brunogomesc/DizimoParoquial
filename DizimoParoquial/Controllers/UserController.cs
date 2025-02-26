using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Text.Json;

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
            List<UserDTO> users = new List<UserDTO>();

            if (TempData["Users"] is string usersJson && !string.IsNullOrEmpty(usersJson))
            {
                users = JsonSerializer.Deserialize<List<UserDTO>>(usersJson);
                TempData.Remove("Users");
            }

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> SearchUser(string status, string name)
        {

            List<UserDTO> users = new List<UserDTO>();

            bool? statusConverted = status != null ? Convert.ToBoolean(status) : null;

            if (status == null && name == null)
                users = await _userService.GetUsersWithouthFilters();

            else
                users = await _userService.GetUsersWithFilters(statusConverted, name);

            TempData["Users"] = JsonSerializer.Serialize(users); // Serializa a lista para TempData

            return RedirectToAction(nameof(Index));
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

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int userId)
        {

            if(userId == 0)
            {
                _notification.AddErrorToastMessage("Dados do usuário não localizado para a exclusão!");
                return RedirectToAction(nameof(Index));
            }

            bool userWasDeleted = await _userService.DeleteUser(userId);

            if(userWasDeleted)
                _notification.AddErrorToastMessage("Usuário excluido com sucesso!");
            else
                _notification.AddErrorToastMessage("Erro ao excluir o usuário!");

            return RedirectToAction(nameof(Index));
        }
    }
}
