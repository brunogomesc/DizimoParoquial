using DizimoParoquial.DTOs;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class ConsultController : Controller
    {

        private const string ROUTE_SCREEN_CONSULT = "/Views/Consult/Index.cshtml";
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

        public async Task<IActionResult> SearchTithes(string name, int tithePayerCode)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            try
            {

                if (string.IsNullOrWhiteSpace(name) && tithePayerCode == 0)
                {
                    _notification.AddErrorToastMessage("Informe o nome ou código do dizimista.");
                    return RedirectToAction(nameof(Index));
                }

                List<TitheDTO> tithes = await _titheService.GetTithesWithFilters(name, tithePayerCode);

                if (tithes == null || tithes.Count == 0)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(Index));
                }

                List<TitheDTO> tithesOrdered = tithes.OrderByDescending(t => t.PaymentMonth).ToList();

                List<TitheDTO> latestTithes = tithesOrdered.Take(3).ToList();

                return View(ROUTE_SCREEN_CONSULT, latestTithes);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }
    }
}
