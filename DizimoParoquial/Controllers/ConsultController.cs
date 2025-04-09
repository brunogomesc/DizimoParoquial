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

                List<TithePayerLaunchDTO> tithePayers = await _titheService.GetTithesWithFilters(name, tithePayerCode, document);

                if (tithePayers == null || tithePayers.Count == 0)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(ConsultAllUsers));
                }

                return View(ROUTE_SCREEN_CONSULT_ALL_USERS, tithePayers);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(ConsultAllUsers));

        }

        [HttpGet]
        public async Task<IActionResult> SearchTithePayerAllUsers(int code)
        {

            try
            {

                if (code == 0)
                {
                    _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                    return RedirectToAction(nameof(ConsultAllUsers));
                }

                List<TitheDTO> tithes = await _titheService.GetTithesByTithePayerId(code);

                if (tithes == null)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(ConsultAllUsers));
                }

                return Json(tithes);
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
