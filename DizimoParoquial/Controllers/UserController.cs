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
        private readonly EventService _eventService;

        public UserController(
            IToastNotification notification, 
            UserService userService,
            EventService eventService)
        {
            _notification = notification;
            _userService = userService;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if (idUser != null && idUser != 0)
            {

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO TELA USUÁRIOS DE ACESSO";

                details = $"{username} acessou tela de usuários de acesso!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchUser(string status, string name, string pageAmount, string page, string buttonPage)
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

                List<UserDTO> users = new List<UserDTO>();

                try
                {

                    bool? statusConverted = status != null ? Convert.ToBoolean(status) : null;

                    if (status == null && name == null)
                        users = await _userService.GetUsersWithouthFilters();

                    else
                        users = await _userService.GetUsersWithFilters(statusConverted, name);

                    ViewBag.UserName = username;

                    #region Paginação

                    int actualPage = 0;
                    List<UserDTO> usersPaginated = new();

                    if (buttonPage != null)
                    {
                        actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                    }
                    else if (page != null)
                    {
                        actualPage = Convert.ToInt32(page.Substring(0, (page.IndexOf("_"))));
                    }

                    int pageSize = pageAmount != null ? Convert.ToInt32(pageAmount) : 10;
                    int count = 0;
                    string action = page is null ? "" : page.Substring(3, page.Length - 3);
                    int totalPages = users.Count % pageSize == 0 ? users.Count / pageSize : (users.Count / pageSize) + 1;
                    ViewBag.TotalPages = totalPages;
                    ViewBag.ActualPage = actualPage;

                    if (action.Contains("back") || action.Contains("next"))
                    {
                        actualPage = action.Contains("back") ? ViewBag.ActualPage - 1 : ViewBag.ActualPage + 1;
                    }
                    else if (buttonPage != null)
                    {
                        actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                    }

                    actualPage = actualPage < 0 ? 0 : actualPage;

                    ViewBag.ActualPage = actualPage;

                    if (users.Count > pageSize)
                    {
                        for (int i = (actualPage * pageSize); i < users.Count; i++)
                        {
                            usersPaginated.Add(users[i]);
                            count++;

                            if (count == pageSize)
                                break;
                        }
                    }
                    else
                    {
                        usersPaginated = users;
                    }

                    TempData["TotalCredenciais"] = users.Count;
                    TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                    TempData["UltimoRegistro"] = users.Count <= pageSize ? users.Count : (actualPage * pageSize) + count;

                    #endregion

                    process = "FILTRAGEM USUÁRIOS DE ACESSO";

                    details = $"{username} filtrou os usuários de acesso!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "FILTRAGEM DE USUÁRIOS DE ACESSO";

                    details = $"{username} filtrou os usuários de acesso. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return View(ROUTE_SCREEN_USERS, users);
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(User user)
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

                try
                {
                    if (!ModelState.IsValid)
                    {
                        _notification.AddErrorToastMessage("Dados de cadastro não estão válidos!");
                        return RedirectToAction(nameof(Index));
                    }

                    bool userWasCreated = await _userService.RegisterUser(user);

                    ViewBag.UserName = username;

                    process = "CRIAÇÃO USUÁRIO DE ACESSO";

                    if (userWasCreated)
                    {
                        _notification.AddSuccessToastMessage("Usuário de acesso criado com sucesso!");

                        details = $"{username} criou o usuário de acesso com sucesso!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Não foi possível criar o usuário de acesso!");

                        details = $"{username} não foi possível criar o usuário de acesso, devido a uma falha!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "CRIAÇÃO USUÁRIO DE ACESSO";

                    details = $"{username} falhou na criação de usuário de acesso. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

                try
                {
                    if (user.UserId == 0 || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Username))
                    {
                        _notification.AddErrorToastMessage("Dados de para alteração não estão válidos!");
                        return RedirectToAction(nameof(Index));
                    }

                    bool userWasUpdated = await _userService.UpdateUser(user);

                    ViewBag.UserName = username;

                    process = "EDIÇÃO USUÁRIO DE ACESSO";

                    if (userWasUpdated)
                    {
                        _notification.AddSuccessToastMessage("Usuário de acesso editado com sucesso!");

                        details = $"{username} editou o usuário de acesso com sucesso!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Não foi possível editar o usuário de acesso!");

                        details = $"{username} não foi possível editar o usuário de acesso, devido a uma falha!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EDIÇÃO USUÁRIO DE ACESSO";

                    details = $"{username} falhou na edição de usuário de acesso. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }


        public async Task<IActionResult> DeleteUser(int userId)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

                try
                {
                    if (userId == 0)
                    {
                        _notification.AddErrorToastMessage("Dados do usuário não localizado para a exclusão!");
                        return RedirectToAction(nameof(Index));
                    }

                    bool userWasDeleted = await _userService.DeleteUser(userId);

                    ViewBag.UserName = username;

                    process = "EXCLUSÃO USUÁRIO DE ACESSO";

                    if (userWasDeleted)
                    {
                        _notification.AddSuccessToastMessage("Usuário de acesso excluido com sucesso!");

                        details = $"{username} excluiu o usuário de acesso com sucesso!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Não foi possível excluir o usuário de acesso!");

                        details = $"{username} não foi possível excluir o usuário de acesso, devido a uma falha!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EXCLUSÃO USUÁRIO DE ACESSO";

                    details = $"{username} falhou na exclusão de usuário de acesso. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
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
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }
    }
}
