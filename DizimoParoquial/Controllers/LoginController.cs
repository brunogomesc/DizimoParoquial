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
        private const string ROUTE_SCREEN_HOME_AGENTS = "/Views/Home/HomeAgents.cshtml";
        private readonly IToastNotification _notification;
        private readonly UserService _userService;
        private readonly AgentService _agentService;
        private readonly EventService _eventService;

        public LoginController(
            IToastNotification notification, 
            UserService userService,
            AgentService agentService,
            EventService eventService)
        {
            _notification = notification;
            _userService = userService;
            _agentService = agentService;
            _eventService = eventService;
        }

        [Route("Gerenciar")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/")]
        public IActionResult LoginAgents()
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
                HttpContext.Session.SetString("Profile", userAuthenticated.Profile);

                ViewBag.UserName = userAuthenticated.Username;

                //bool eventRegistered = await _eventService.SaveEvent();

                _notification.AddSuccessToastMessage("Usuário autenticado com sucesso!");

                return View(ROUTE_SCREEN_HOME);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> LoginAgentsAutentication(string codeAgent, string phonenumber)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(codeAgent) || string.IsNullOrWhiteSpace(phonenumber))
                {
                    _notification.AddErrorToastMessage("Código do Agente e/ou Telefone não preenchidos!");
                    return RedirectToAction(nameof(LoginAgents));
                }

                AgentDTO agentAuthenticated = await _agentService.GetAgentByCode(codeAgent);

                if (agentAuthenticated == null 
                    || agentAuthenticated.AgentId == 0
                    || agentAuthenticated.PhoneNumber != phonenumber)
                {
                    _notification.AddErrorToastMessage("Agente do dizimo não está válido ou está inativo!");
                    return RedirectToAction(nameof(LoginAgents));
                }

                HttpContext.Session.SetInt32("Agent", agentAuthenticated.AgentId);
                HttpContext.Session.SetString("AgentName", agentAuthenticated.Name);
                HttpContext.Session.SetString("AgentCode", agentAuthenticated.AgentCode);

                _notification.AddSuccessToastMessage("Agente autenticado com sucesso!");

                return View(ROUTE_SCREEN_HOME_AGENTS);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(LoginAgents));

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

    }
}
