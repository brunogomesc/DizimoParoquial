using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Globalization;

namespace DizimoParoquial.Controllers
{
    public class LaunchController : Controller
    {

        private const string ROUTE_SCREEN_LAUNCH_ALLUSERS = "/Views/Launch/LaunchAllUsers.cshtml";
        private const string ROUTE_SCREEN_LAUNCH = "/Views/Launch/LaunchAllUsers.cshtml";
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
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        #region Public Methods - All Users

        [HttpGet]
        public async Task<IActionResult> SearchTithePayerAllUsers(int code)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

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

        public async Task<IActionResult> SearchTithePayerLaunchAllUsers(string name, int code)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

                if (name == null && code == 0)
                {
                    _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                    return RedirectToAction(nameof(LaunchAllUsers));
                }

                List<TithePayerLaunchDTO> tithePayer = await _tithePayerService.GetTithePayersLauchingWithFilters(name, code);

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

            ViewBag.UserName = HttpContext.Session.GetString("Username");

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
                    _notification.AddErrorToastMessage("Data de Pagamento é obrigatório!");
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

    }
}