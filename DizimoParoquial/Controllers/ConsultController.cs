using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DizimoParoquial.Controllers
{
    public class ConsultController : Controller
    {

        private const string ROUTE_SCREEN_CONSULT_ALL_USERS = "/Views/Consult/ConsultAllUsers.cshtml";
        private const string ROUTE_SCREEN_CONSULT = "/Views/Consult/Index.cshtml";
        private readonly IToastNotification _notification;
        private readonly TitheService _titheService;
        private readonly EventService _eventService;
        private readonly AgentService _agentService;

        public ConsultController(
            IToastNotification notification, 
            TitheService titheService,
            EventService eventService,
            AgentService agentService)
        {
            _notification = notification;
            _titheService = titheService;
            _eventService = eventService;
            _agentService = agentService;
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

                process = "ACESSO TELA CONSULTAS DIZIMO";

                details = $"{username} acessou tela de consulta de dizimos!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        [Route("ConsultarDizimo")]
        public async Task<IActionResult> ConsultAllUsers()
        {

            string? process, details;
            bool eventRegistered;

            int? agentId = HttpContext.Session.GetInt32("Agent");

            if (agentId != null && agentId != 0)
            {
                string? agentCode = HttpContext.Session.GetString("AgentCode");

                process = "ACESSO TELA CONSULTAS DIZIMO";

                string? agentName = HttpContext.Session.GetString("AgentName");

                details = $"{agentName} acessou tela de consulta de dizimos!";

                eventRegistered = await _eventService.SaveEvent(process, details, agentId: agentId);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("LoginAgents", "Login");
            }
        }

        #region Public Methods - All Users

        public async Task<IActionResult> SearchTithesAllUsers(string name, int tithePayerCode, string document, string pageAmount, string page, string buttonPage)
        {

            string? process, details;
            bool eventRegistered;
            
            string? agentCode = HttpContext.Session.GetString("AgentCode");
            string? agentName = HttpContext.Session.GetString("AgentName");
            int? agentId = HttpContext.Session.GetInt32("Agent");

            if(agentId != null && agentId != 0)
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(name) && tithePayerCode == 0 && string.IsNullOrWhiteSpace(document))
                    {
                        _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                        return RedirectToAction(nameof(ConsultAllUsers));
                    }

                    if (document != null)
                        document = document.Replace(".", "").Replace("-", "");

                    List<TithePayerLaunchDTO> tithePayers = await _titheService.GetTithesWithFilters(name, tithePayerCode, document);

                    if (tithePayers == null || tithePayers.Count == 0)
                    {
                        _notification.AddErrorToastMessage("Dizimista não encontrado.");
                        return RedirectToAction(nameof(ConsultAllUsers));
                    }

                    #region Paginação

                    int actualPage = 0;
                    List<TithePayerLaunchDTO> tithePayersPaginated = new();

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
                    int totalPages = tithePayers.Count % pageSize == 0 ? tithePayers.Count / pageSize : (tithePayers.Count / pageSize) + 1;
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

                    if (tithePayers.Count > pageSize)
                    {
                        for (int i = (actualPage * pageSize); i < tithePayers.Count; i++)
                        {
                            tithePayersPaginated.Add(tithePayers[i]);
                            count++;

                            if (count == pageSize)
                                break;
                        }
                    }
                    else
                    {
                        tithePayersPaginated = tithePayers;
                    }

                    TempData["TotalCredenciais"] = tithePayers.Count;
                    TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                    TempData["UltimoRegistro"] = tithePayers.Count <= pageSize ? tithePayers.Count : (actualPage * pageSize) + count;

                    #endregion

                    process = "CONSULTA DE DIZIMISTAS";

                    details = $"{agentName} acessou tela de consulta de dizimos!";

                    eventRegistered = await _eventService.SaveEvent(process, details, agentId: agentId);

                    return View(ROUTE_SCREEN_CONSULT_ALL_USERS, tithePayersPaginated);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "CONSULTA DE DIZIMISTAS";

                    details = $"{agentName} falhou na pesquisa de dizimistas. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(ConsultAllUsers));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("LoginAgents", "Login");
            }

        }

        [HttpGet]
        public async Task<IActionResult> SearchTithePayerAllUsers(int code)
        {

            int? agentId = HttpContext.Session.GetInt32("Agent");

            if (agentId != null && agentId != 0)
            {
                try
                {

                    if (code == 0)
                    {
                        _notification.AddErrorToastMessage("Informe o código do dizimista.");
                        return RedirectToAction(nameof(ConsultAllUsers));
                    }

                    List<TitheDTO> tithes = await _titheService.GetTithesByTithePayerId(code);

                    if (tithes == null)
                    {
                        _notification.AddErrorToastMessage("Dizimista não encontrado.");
                        return RedirectToAction(nameof(ConsultAllUsers));
                    }

                    List<TitheDTO> tithesOrganized = tithes.OrderByDescending(t => t.PaymentMonth).ToList();

                    return Json(tithesOrganized.Take(3));
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);
                }

                return RedirectToAction(nameof(ConsultAllUsers));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("LoginAgents", "Login");
            }

        }

        #endregion

        #region Methods - User Authenticated

        public async Task<IActionResult> SearchTithes(string name, int tithePayerCode, string document, string pageAmount, string page, string buttonPage)
        {
            string? process, details, username;
            bool eventRegistered;

            int? userId = HttpContext.Session.GetInt32("User");
            username = HttpContext.Session.GetString("Username");

            if(userId != null && userId != 0)
            {
                try
                {

                    ViewBag.UserName = username;

                    if (string.IsNullOrWhiteSpace(name) && tithePayerCode == 0 && string.IsNullOrWhiteSpace(document))
                    {
                        _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                        return RedirectToAction(nameof(Index));
                    }

                    if (document != null)
                        document = document.Replace(".", "").Replace("-", "");

                    List<TithePayerLaunchDTO> tithePayers = await _titheService.GetTithesWithFilters(name, tithePayerCode, document);

                    if (tithePayers == null || tithePayers.Count == 0)
                    {
                        _notification.AddErrorToastMessage("Dizimista não encontrado.");
                        return RedirectToAction(nameof(Index));
                    }

                    #region Paginação

                    int actualPage = 0;
                    List<TithePayerLaunchDTO> tithePayersPaginated = new();

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
                    int totalPages = tithePayers.Count % pageSize == 0 ? tithePayers.Count / pageSize : (tithePayers.Count / pageSize) + 1;
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

                    if (tithePayers.Count > pageSize)
                    {
                        for (int i = (actualPage * pageSize); i < tithePayers.Count; i++)
                        {
                            tithePayersPaginated.Add(tithePayers[i]);
                            count++;

                            if (count == pageSize)
                                break;
                        }
                    }
                    else
                    {
                        tithePayersPaginated = tithePayers;
                    }

                    TempData["TotalCredenciais"] = tithePayers.Count;
                    TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                    TempData["UltimoRegistro"] = tithePayers.Count <= pageSize ? tithePayers.Count : (actualPage * pageSize) + count;

                    #endregion

                    process = "CONSULTA DE DIZIMISTAS";

                    details = $"{username} realizou a consulta de dizimistas!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: userId);

                    return View(ROUTE_SCREEN_CONSULT, tithePayersPaginated);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "CONSULTA DE DIZIMISTAS";

                    details = $"{username} falhou na consulta de dizimistas. Erro: {ex.Message}";

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
        public async Task<IActionResult> SearchTithePayer(int code)
        {

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                try
                {

                    if (code == 0)
                    {
                        _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                        return RedirectToAction(nameof(Index));
                    }

                    List<TitheDTO> tithes = await _titheService.GetTithesByTithePayerId(code);

                    if (tithes == null)
                    {
                        _notification.AddErrorToastMessage("Dizimista não encontrado.");
                        return RedirectToAction(nameof(Index));
                    }

                    List<TitheDTO> tithesOrganized = tithes.OrderByDescending(t => t.PaymentMonth).ToList();

                    return Json(tithesOrganized.Take(3));
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

        #endregion

    }
}
