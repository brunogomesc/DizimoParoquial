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

            string process, details;
            bool eventRegistered;

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

                    process = "LOGIN USUÁRIO";

                    details = $"{user} falhou no login. Erro: Usuário não é válido ou está inativo!";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                    return RedirectToAction(nameof(Index));
                }

                HttpContext.Session.SetInt32("User", userAuthenticated.UserId);
                HttpContext.Session.SetString("Username", userAuthenticated.Username);
                HttpContext.Session.SetString("Profile", userAuthenticated.Profile);
                HttpContext.Session.SetInt32("SuperUser", userAuthenticated.SuperUser ? 1 : 0);

                ViewBag.UserName = userAuthenticated.Username;

                process = "LOGIN USUÁRIO";

                details = $"{userAuthenticated.Username} autenticado com sucesso!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: userAuthenticated.UserId);

                _notification.AddSuccessToastMessage($"Bem Vindo, {userAuthenticated.Name}!");

                return View(ROUTE_SCREEN_HOME);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);

                process = "LOGIN USUÁRIO";

                details = $"{user} falhou no login. Erro: {ex.Message}";

                eventRegistered = await _eventService.SaveEvent(process, details);

            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> LoginAgentsAutentication(string codeAgent, string phonenumber)
        {

            string process, details;
            bool eventRegistered;

            try
            {
                if (string.IsNullOrWhiteSpace(codeAgent) || string.IsNullOrWhiteSpace(phonenumber))
                {
                    _notification.AddErrorToastMessage("Código do Agente e/ou Telefone não preenchidos!");

                    process = "LOGIN AGENTE";

                    details = $"Código do Agente: {codeAgent} - falhou no login. Erro: Código do Agente e/ou Telefone não preenchidos!";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                    return RedirectToAction(nameof(LoginAgents));
                }

                AgentDTO agentAuthenticated = await _agentService.GetAgentByCode(codeAgent);

                if (agentAuthenticated == null 
                    || agentAuthenticated.AgentId == 0
                    || agentAuthenticated.PhoneNumber != phonenumber)
                {
                    _notification.AddErrorToastMessage("Agente do dizimo não está válido ou está inativo!");

                    process = "LOGIN AGENTE";

                    details = $"Código do Agente: {codeAgent} - falhou no login. Erro: Agente do dizimo não está válido ou está inativo!";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                    return RedirectToAction(nameof(LoginAgents));
                }

                HttpContext.Session.SetInt32("Agent", agentAuthenticated.AgentId);
                HttpContext.Session.SetString("AgentName", agentAuthenticated.Name);
                HttpContext.Session.SetString("AgentCode", agentAuthenticated.AgentCode);

                process = "LOGIN AGENTE";

                details = $"{agentAuthenticated.Name} autenticado com sucesso!";

                eventRegistered = await _eventService.SaveEvent(process, details, agentId: agentAuthenticated.AgentId);

                _notification.AddSuccessToastMessage($"Bem Vindo, {agentAuthenticated.Name}!");

                return View(ROUTE_SCREEN_HOME_AGENTS);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);

                process = "LOGIN AGENTE";

                details = $"Código de agente: {codeAgent} - falhou no login. Erro: {ex.Message}";

                eventRegistered = await _eventService.SaveEvent(process, details);

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
