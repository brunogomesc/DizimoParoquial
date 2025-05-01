using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils.Filters;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
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

        public LaunchController(
            IToastNotification notification,
            TithePayerService tithePayerService,
            TitheService titheService,
            AgentService agentService)
        {
            _notification = notification;
            _tithePayerService = tithePayerService;
            _titheService = titheService;
            _agentService = agentService;
        }

        //[SessionAuthorize("REGTIT")]
        [Route("SalvarDizimo")]
        public IActionResult LaunchAllUsers()
        {

            string? agentCode = HttpContext.Session.GetString("AgentCode");

            if (string.IsNullOrWhiteSpace(agentCode))
            {
                _notification.AddErrorToastMessage("Sessão do usuário expirou. Conecte-se novamente!");
                RedirectToAction("LoginAgents", "Login");
            }

            return View();
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult EditLaunch()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
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

        public async Task<IActionResult> SearchTithePayerLaunchAllUsers(string name, int tithePayerCode)
        {

            try
            {

                if (name == null && tithePayerCode == 0)
                {
                    _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                    return RedirectToAction(nameof(LaunchAllUsers));
                }

                List<TithePayerLaunchDTO> tithePayer = await _tithePayerService.GetTithePayersLauchingWithFilters(name, tithePayerCode);

                if (tithePayer == null)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(LaunchAllUsers));
                }

                return View(ROUTE_SCREEN_LAUNCH_ALLUSERS, tithePayer);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(LaunchAllUsers));

        }

        public async Task<IActionResult> SaveTitheAllUsers(string value, string agentCode, string paymentType, int tithePayerId, DateTime[] dates)
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
                    _notification.AddSuccessToastMessage("Dizimo registrado com sucesso!");
                else
                    _notification.AddErrorToastMessage("Não foi possível registrar o dizimo!");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(LaunchAllUsers));
        }

        #endregion

        #region Methods - User Authenticated

        [HttpGet]
        public async Task<IActionResult> SearchTithePayer(int code)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

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

        public async Task<IActionResult> SearchTithePayerLaunch(string name, int tithePayerCode)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

                if (name == null && tithePayerCode == 0)
                {
                    _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                    return RedirectToAction(nameof(Index));
                }

                List<TithePayerLaunchDTO> tithePayer = await _tithePayerService.GetTithePayersLauchingWithFilters(name, tithePayerCode);

                if (tithePayer == null)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(Index));
                }

                return View(ROUTE_SCREEN_LAUNCH, tithePayer);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> SaveTithe(string value, string agentCode, string paymentType, int tithePayerId, DateTime[] dates)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

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
                    _notification.AddSuccessToastMessage("Dizimo registrado com sucesso!");
                else
                    _notification.AddErrorToastMessage("Não foi possível registrar o dizimo!");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SearchTithesEditLaunch(int tithePayerCode, string document)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

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

                var tithesOrganized = tithes.OrderByDescending(t => t.PaymentMonth).ToList();

                return View(ROUTE_SCREEN_EDIT_LAUNCH, tithesOrganized);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(EditLaunch));

        }

        [HttpGet]
        public async Task<IActionResult> GetDetailsEdit(int id)
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

        public async Task<IActionResult> SaveEditLaunch(int tithePayerEdit, string agentEdit, string paymentTypeEdit, string valueEdit, DateTime datesEdit, int titheIdEdit)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
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

                if (titheWasUpdated)
                    _notification.AddSuccessToastMessage("Dizimo atualizado com sucesso!");
                else
                    _notification.AddErrorToastMessage("Não foi possível atualizar o dizimo!");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(EditLaunch));
        }

        #endregion

    }
}