using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Text.Json;

namespace DizimoParoquial.Controllers
{
    //[SessionAuthorize("MANUSER")]
    public class UserController : Controller
    {

        private const string ROUTE_SCREEN_USERS = "/Views/User/Index.cshtml";
        private readonly IToastNotification _notification;
        private readonly UserService _userService;

        public UserController(IToastNotification notification, UserService userService)
        {
            _notification = notification;
            _userService = userService;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchUser(string status, string name)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");
            List<UserDTO> users = new List<UserDTO>();

            try
            {

                bool? statusConverted = status != null ? Convert.ToBoolean(status) : null;

                if (status == null && name == null)
                    users = await _userService.GetUsersWithouthFilters();

                else
                    users = await _userService.GetUsersWithFilters(statusConverted, name);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return View(ROUTE_SCREEN_USERS, users);

        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(User user)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
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

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {
                if (user.UserId == 0 || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Username))
                {
                    _notification.AddErrorToastMessage("Dados de para alteração não estão válidos!");
                    return RedirectToAction(nameof(Index));
                }

                bool userWasUpdated = await _userService.UpdateUser(user);

                if (userWasUpdated)
                    _notification.AddSuccessToastMessage("Usuário alterado com sucesso!");
                else
                    _notification.AddErrorToastMessage("Não foi possível alterar o usuário!");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteUser(int userId)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {
                if (userId == 0)
                {
                    _notification.AddErrorToastMessage("Dados do usuário não localizado para a exclusão!");
                    return RedirectToAction(nameof(Index));
                }

                bool userWasDeleted = await _userService.DeleteUser(userId);

                if (userWasDeleted)
                    _notification.AddSuccessToastMessage("Usuário excluido com sucesso!");
                else
                    _notification.AddErrorToastMessage("Erro ao excluir o usuário!");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {
                UserDTO user = await _userService.GetUserById(id);

                if (user == null)
                {
                    _notification.AddErrorToastMessage("Usuário não localizado!");
                    return RedirectToAction(nameof(Index));
                }

                return Json(user);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }
    }
}
