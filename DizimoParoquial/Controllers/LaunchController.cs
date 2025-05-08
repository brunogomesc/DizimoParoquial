using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils.Filters;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Diagnostics;
using System.Globalization;

namespace DizimoParoquial.Controllers
{
    public class LaunchController : Controller
    {

        private const string ROUTE_SCREEN_LAUNCH_ALLUSERS = "/Views/Launch/LaunchAllUsers.cshtml";
        private const string ROUTE_SCREEN_LAUNCH = "/Views/Launch/Index.cshtml";
        private const string ROUTE_SCREEN_EDIT_LAUNCH = "/Views/Launch/EditLaunch.cshtml";
        private readonly IToastNotification _notification;
        private readonly TithePayerService _tithePayerService;
        private readonly TitheService _titheService;
        private readonly AgentService _agentService;
        private readonly EventService _eventService;

        public LaunchController(
            IToastNotification notification,
            TithePayerService tithePayerService,
            TitheService titheService,
            AgentService agentService,
            EventService eventService)
        {
            _notification = notification;
            _tithePayerService = tithePayerService;
            _titheService = titheService;
            _agentService = agentService;
            _eventService = eventService;
        }

        //[SessionAuthorize("REGTIT")]
        [Route("SalvarDizimo")]
        public async Task<IActionResult> LaunchAllUsers()
        {

            string? process, details;
            bool eventRegistered;

            int? agentId = HttpContext.Session.GetInt32("Agent");

            if (agentId != null && agentId != 0)
            {
                string? agentCode = HttpContext.Session.GetString("AgentCode");

                process = "ACESSO TELA LANÇAMENTO DIZIMO";

                string? agentName = HttpContext.Session.GetString("AgentName");
                
                details = $"{agentName} acessou tela de lançamento de dizimos!";

                eventRegistered = await _eventService.SaveEvent(process, details, agentId: agentId);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("LoginAgents", "Login");
            }
        }

        public async Task<IActionResult> Index()
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO TELA LANÇAMENTO DIZIMO";

                details = $"{username} acessou tela de lançamento de dizimos!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> EditLaunch()
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO TELA EDIÇÃO DIZIMO";

                details = $"{username} acessou tela de edição de dizimos!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();

            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        #region Public Methods - All Users

        [HttpGet]
        public async Task<IActionResult> SearchTithePayerAllUsers(int code)
        {

            try
            {

                if (code == 0)
                {
                    _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                    return RedirectToAction(nameof(LaunchAllUsers));
                }

                TithePayerLaunchDTO tithePayer = await _tithePayerService.GetTithePayersLauchingWithFilters(code);

                if (tithePayer == null)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(LaunchAllUsers));
                }

                return Json(tithePayer);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(LaunchAllUsers));

        }

        public async Task<IActionResult> SearchTithePayerLaunchAllUsers(string name, int tithePayerCode, string pageAmount, string page, string buttonPage)
        {

            int? agentId = HttpContext.Session.GetInt32("Agent");

            if (agentId != null && agentId != 0)
            {
                try
                {

                    if (name == null && tithePayerCode == 0)
                    {
                        _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                        return RedirectToAction(nameof(LaunchAllUsers));
                    }

                    List<TithePayerLaunchDTO> tithePayers = await _tithePayerService.GetTithePayersLauchingWithFilters(name, tithePayerCode);

                    if (tithePayers == null)
                    {
                        _notification.AddErrorToastMessage("Dizimista não encontrado.");
                        return RedirectToAction(nameof(LaunchAllUsers));
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

                    return View(ROUTE_SCREEN_LAUNCH_ALLUSERS, tithePayersPaginated);
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);
                }

                return RedirectToAction(nameof(LaunchAllUsers));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("LoginAgents", "Login");
            }

        }

        public async Task<IActionResult> SaveTitheAllUsers(string value, string agentCode, string paymentType, int tithePayerId, DateTime[] dates)
        {

            string? process, details, username;
            bool eventRegistered;

            string? code = HttpContext.Session.GetString("AgentCode");
            string? agentName = HttpContext.Session.GetString("AgentName");
            int? agentId = HttpContext.Session.GetInt32("Agent");

            if(agentId != null && agentId != 0)
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _notification.AddErrorToastMessage("Valor é obrigatório!");
                        return RedirectToAction(nameof(LaunchAllUsers));
                    }

                    if (string.IsNullOrWhiteSpace(agentCode))
                    {
                        _notification.AddErrorToastMessage("Código do Agente do Dizimo é obrigatório!");
                        return RedirectToAction(nameof(LaunchAllUsers));
                    }

                    if (string.IsNullOrWhiteSpace(paymentType))
                    {
                        _notification.AddErrorToastMessage("Tipo de Pagamento é obrigatório!");
                        return RedirectToAction(nameof(LaunchAllUsers));
                    }

                    if (dates == null || dates.Length == 0)
                    {
                        _notification.AddErrorToastMessage("Data de Contribuição é obrigatório!");
                        return RedirectToAction(nameof(LaunchAllUsers));
                    }

                    if (tithePayerId == 0)
                    {
                        _notification.AddErrorToastMessage("Dizimista é obrigatório!");
                        return RedirectToAction(nameof(LaunchAllUsers));
                    }

                    AgentDTO agent = await _agentService.GetAgentByCode(agentCode);

                    if (agent == null || !agent.Active)
                    {
                        _notification.AddErrorToastMessage("Agente do Dizimo não localizado ou inativo!");
                        return RedirectToAction(nameof(LaunchAllUsers));
                    }

                    decimal decimalValue;
                    CultureInfo culturaBrasileira = new CultureInfo("pt-BR");

                    decimal.TryParse(value, NumberStyles.Currency, culturaBrasileira, out decimalValue);

                    LauchingTithe tithe = new LauchingTithe
                    {
                        Value = decimalValue,
                        AgentCode = agentCode,
                        PaymentType = paymentType,
                        PaymentDates = dates,
                        TithePayerId = tithePayerId
                    };

                    bool titheWasSaved = await _titheService.SaveTithe(tithe);

                    if (titheWasSaved)
                    {

                        _notification.AddSuccessToastMessage("Dizimo registrado com sucesso!");

                        process = "LANÇAMENTO DIZIMO";

                        details = $"{agentName} lançou o dizimo com sucesso do dizimista de código: {tithePayerId}.";

                        eventRegistered = await _eventService.SaveEvent(process, details, agentId: agentId);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Não foi possível registrar o dizimo!");

                        process = "LANÇAMENTO DIZIMO";

                        details = $"{agentName} não lançou o dizimo, devido a uma falha!";

                        eventRegistered = await _eventService.SaveEvent(process, details, agentId: agentId);

                    }

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "LANÇAMENTO DE DIZIMO";

                    details = $"{agentName} falhou no lançamento de dizimo. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(LaunchAllUsers));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("LoginAgents", "Login");
            }

        }

        #endregion

        #region Methods - User Authenticated

        [HttpGet]
        public async Task<IActionResult> SearchTithePayer(int code)
        {

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                try
                {

                    ViewBag.UserName = HttpContext.Session.GetString("Username");

                    if (code == 0)
                    {
                        _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                        return RedirectToAction(nameof(Index));
                    }

                    TithePayerLaunchDTO tithePayer = await _tithePayerService.GetTithePayersLauchingWithFilters(code);

                    if (tithePayer == null)
                    {
                        _notification.AddErrorToastMessage("Dizimista não encontrado.");
                        return RedirectToAction(nameof(Index));
                    }

                    return Json(tithePayer);
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

        public async Task<IActionResult> SearchTithePayerLaunch(string name, int tithePayerCode, string pageAmount, string page, string buttonPage)
        {

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                try
                {

                    ViewBag.UserName = HttpContext.Session.GetString("Username");

                    if (name == null && tithePayerCode == 0)
                    {
                        _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                        return RedirectToAction(nameof(Index));
                    }

                    List<TithePayerLaunchDTO> tithePayers = await _tithePayerService.GetTithePayersLauchingWithFilters(name, tithePayerCode);

                    if (tithePayers == null)
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

                    return View(ROUTE_SCREEN_LAUNCH, tithePayersPaginated);
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

        public async Task<IActionResult> SaveTithe(string value, string agentCode, string paymentType, int tithePayerId, DateTime[] dates)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _notification.AddErrorToastMessage("Valor é obrigatório!");
                        return RedirectToAction(nameof(Index));
                    }

                    if (string.IsNullOrWhiteSpace(agentCode))
                    {
                        _notification.AddErrorToastMessage("Código do Agente do Dizimo é obrigatório!");
                        return RedirectToAction(nameof(Index));
                    }

                    if (string.IsNullOrWhiteSpace(paymentType))
                    {
                        _notification.AddErrorToastMessage("Tipo de Pagamento é obrigatório!");
                        return RedirectToAction(nameof(Index));
                    }

                    if (dates == null || dates.Length == 0)
                    {
                        _notification.AddErrorToastMessage("Data de Contribuição é obrigatório!");
                        return RedirectToAction(nameof(Index));
                    }

                    if (tithePayerId == 0)
                    {
                        _notification.AddErrorToastMessage("Dizimista é obrigatório!");
                        return RedirectToAction(nameof(Index));
                    }

                    AgentDTO agent = await _agentService.GetAgentByCode(agentCode);

                    if (agent == null || !agent.Active)
                    {
                        _notification.AddErrorToastMessage("Agente do Dizimo não localizado ou inativo!");
                        return RedirectToAction(nameof(Index));
                    }

                    decimal decimalValue;
                    CultureInfo culturaBrasileira = new CultureInfo("pt-BR");

                    decimal.TryParse(value, NumberStyles.Currency, culturaBrasileira, out decimalValue);

                    DateTime[] formattedDates = new DateTime[dates.Length];
                    int index = 0;

                    foreach (DateTime data in dates)
                    {
                        formattedDates[index] = data.Date;
                        index++;
                    }

                    LauchingTithe tithe = new LauchingTithe
                    {
                        Value = decimalValue,
                        AgentCode = agentCode,
                        PaymentType = paymentType,
                        PaymentDates = formattedDates,
                        TithePayerId = tithePayerId
                    };

                    bool titheWasSaved = await _titheService.SaveTithe(tithe);

                    ViewBag.UserName = username;

                    process = "LANÇAMENTO DIZIMO";

                    if (titheWasSaved)
                    {
                        _notification.AddSuccessToastMessage("Dizimo registrado com sucesso!");

                        details = $"{username} lançou o dizimo com sucesso, do dizimista de código: {tithePayerId}.";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Não foi possível registrar o dizimo!");

                        details = $"{username} não foi possível lançar o dizimo, devido a uma falha!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "LANÇAMENTO DIZIMO";

                    details = $"{username} falhou no lançamento do dizimo. Erro: {ex.Message}";

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

        public async Task<IActionResult> SearchTithesEditLaunch(int tithePayerCode, string document, string pageAmount, string page, string buttonPage)
        {

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                try
                {

                    ViewBag.UserName = HttpContext.Session.GetString("Username");

                    if (tithePayerCode == 0 && string.IsNullOrWhiteSpace(document))
                    {
                        _notification.AddErrorToastMessage("Informe o documento ou código do dizimista.");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    if (document != null)
                        document = document.Replace(".", "").Replace("-", "");

                    List<TithePayerLaunchDTO> tithePayers = await _titheService.GetTithesWithFilters(null, tithePayerCode, document);

                    if (tithePayers == null || tithePayers.Count == 0)
                    {
                        _notification.AddErrorToastMessage("Dizimista não encontrado.");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    List<TitheDTO> tithes = await _titheService.GetTithesByTithePayerId(tithePayers.First().TithePayerId);

                    #region Paginação

                    int actualPage = 0;
                    List<TitheDTO> tithePayersPaginated = new();

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
                    int totalPages = tithes.Count % pageSize == 0 ? tithes.Count / pageSize : (tithes.Count / pageSize) + 1;
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

                    if (tithes.Count > pageSize)
                    {
                        for (int i = (actualPage * pageSize); i < tithes.Count; i++)
                        {
                            tithePayersPaginated.Add(tithes[i]);
                            count++;

                            if (count == pageSize)
                                break;
                        }
                    }
                    else
                    {
                        tithePayersPaginated = tithes;
                    }

                    TempData["TotalCredenciais"] = tithes.Count;
                    TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                    TempData["UltimoRegistro"] = tithes.Count <= pageSize ? tithes.Count : (actualPage * pageSize) + count;

                    #endregion

                    var tithesOrganized = tithePayersPaginated.OrderByDescending(t => t.PaymentMonth).ToList();

                    return View(ROUTE_SCREEN_EDIT_LAUNCH, tithesOrganized);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);
                }

                return RedirectToAction(nameof(EditLaunch));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetDetailsEdit(int id)
        {

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                ViewBag.UserName = HttpContext.Session.GetString("Username");

                try
                {
                    TitheDTO tithe = await _titheService.GetTitheById(id);

                    if (tithe == null)
                    {
                        _notification.AddErrorToastMessage("Dizimo não localizado!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    return Json(tithe);
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);
                }

                return RedirectToAction(nameof(EditLaunch));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        public async Task<IActionResult> SaveEditLaunch(int tithePayerEdit, string agentEdit, string paymentTypeEdit, string valueEdit, DateTime datesEdit, int titheIdEdit)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {
                ViewBag.UserName = username;
                try
                {

                    if (string.IsNullOrWhiteSpace(valueEdit))
                    {
                        _notification.AddErrorToastMessage("Valor é obrigatório!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    if (string.IsNullOrWhiteSpace(agentEdit))
                    {
                        _notification.AddErrorToastMessage("Agente do Dizimo é obrigatório!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    if (string.IsNullOrWhiteSpace(paymentTypeEdit))
                    {
                        _notification.AddErrorToastMessage("Tipo de Pagamento é obrigatório!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    if (datesEdit == DateTime.MinValue)
                    {
                        _notification.AddErrorToastMessage("Data de Contribuição é obrigatório!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    if (tithePayerEdit == 0)
                    {
                        _notification.AddErrorToastMessage("Dizimista é obrigatório!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    TitheDTO? tithe = await _titheService.GetTitheById(titheIdEdit);

                    List<AgentDTO> agents = await _agentService.GetAgentsWithouthFilters();

                    AgentDTO? agent = agents.FirstOrDefault(a => a.Name == agentEdit);

                    if (agent == null || !agent.Active)
                    {
                        _notification.AddErrorToastMessage("Agente do Dizimo não localizado ou inativo!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    if (tithe == null)
                    {
                        _notification.AddErrorToastMessage("Dizimo não localizado!");
                        return RedirectToAction(nameof(EditLaunch));
                    }

                    decimal decimalValue;
                    CultureInfo culturaBrasileira = new CultureInfo("pt-BR");

                    decimal.TryParse(valueEdit, NumberStyles.Currency, culturaBrasileira, out decimalValue);

                    Tithe titheUpdated = new Tithe
                    {
                        AgentCode = agent.AgentCode,
                        PaymentMonth = datesEdit,
                        PaymentType = paymentTypeEdit,
                        TitheId = titheIdEdit,
                        TithePayerId = tithePayerEdit,
                        Value = decimalValue
                    };

                    bool titheWasUpdated = await _titheService.UpdateTithe(titheUpdated);

                    process = "EDIÇÃO DIZIMO";

                    if (titheWasUpdated)
                    {
                        _notification.AddSuccessToastMessage("Dizimo editado com sucesso!");

                        details = $"{username} editou o dizimo com sucesso, do dizimista de código: {tithePayerEdit}.";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Não foi possível editar o dizimo!");

                        details = $"{username} não foi possível editar o dizimo, devido a uma falha!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EDIÇÃO DIZIMO";

                    details = $"{username} falhou na edição do dizimo. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(EditLaunch));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        #endregion


        public async Task<IActionResult> GetAvailableDates(int tithePayerId)
        {
            List<DateTime> dates = await GetDateTimes(tithePayerId);

            List<DateTime> datesNotDuplicate = dates.Distinct().ToList();

            return Json(datesNotDuplicate);
        }

        private async Task<List<DateTime>> GetDateTimes(int tithePayerId)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            var tithes = await _titheService.GetTithesByTithePayerId(tithePayerId);

            for (int i = 6; i > 0; i--)
                dates.Add(today.AddMonths(-i));

            dates.Add(today);

            for (int i = 1; i <= 6; i++)
                dates.Add(today.AddMonths(i));

            List<DateTime> datesAllowed = new List<DateTime>();

            foreach (var tithe in tithes)
            {
                if(dates.Contains(tithe.PaymentMonth))
                {
                    dates.Remove(tithe.PaymentMonth);
                }
            }

            return dates;
        }
    }

}