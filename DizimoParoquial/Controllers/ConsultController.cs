using DizimoParoquial.DTOs;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class ConsultController : Controller
    {

        private const string ROUTE_SCREEN_CONSULT_ALL_USERS = "/Views/Consult/ConsultAllUsers.cshtml";
        private readonly IToastNotification _notification;
        private readonly TitheService _titheService;

        public ConsultController(IToastNotification notification, TitheService titheService)
        {
            _notification = notification;
            _titheService = titheService;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        [Route("ConsultarDizimo")]
        public IActionResult ConsultAllUsers()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        #region Public Methods - All Users

        public async Task<IActionResult> SearchTithesAllUsers(string name, int tithePayerCode, string document)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

                if (string.IsNullOrWhiteSpace(name) && tithePayerCode == 0 && string.IsNullOrWhiteSpace(document))
                {
                    _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                    return RedirectToAction(nameof(ConsultAllUsers));
                }

                if(document != null)
                    document = document.Replace(".", "").Replace("-", "");

                List<TitheDTO> tithes = await _titheService.GetTithesWithFilters(name, tithePayerCode, document);

                if (tithes == null || tithes.Count == 0)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(ConsultAllUsers));
                }

                List<TitheDTO> tithesOrdered = tithes.OrderByDescending(t => t.PaymentMonth).ToList();

                List<TitheDTO> latestTithes = tithesOrdered.Take(3).ToList();

                return View(ROUTE_SCREEN_CONSULT_ALL_USERS, latestTithes);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(ConsultAllUsers));

        }

        #endregion

    }
}
