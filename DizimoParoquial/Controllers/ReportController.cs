using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class ReportController : Controller
    {

        private const string ROUTE_SCREEN_REPORTS = "/Views/Report/ReportTithePayer.cshtml";
        private const string ROUTE_SCREEN_TITHE_PER_TITHEPAYER = "/Views/Report/ReportTithePayerPerTithe.cshtml";
        private const string ROUTE_SCREEN_BIRTHDAYS = "/Views/Report/ReportBirthdays.cshtml";
        
        private readonly IToastNotification _notification;
        private readonly TithePayerService _tithePayer;
        private readonly TitheService _titheService;

        public ReportController(
            IToastNotification notification, 
            TithePayerService tithePayer,
            TitheService titheService)
        {
            _notification = notification;
            _tithePayer = tithePayer;
            _titheService = titheService;
        }

        #region Views

        public IActionResult ReportValuePerPaymentType()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult ReportTithePayer()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult ReportTithePayerPerTithe()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult ReportBirthdays()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        #endregion

        public async Task<IActionResult> SearchReportTithePayer(string paymentType, string name, DateTime startPaymentDate, DateTime endPaymentDate)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");

            if(startPaymentDate == DateTime.MinValue)
            {
                startPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if(endPaymentDate == DateTime.MinValue)
            {
                endPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            }

            if(startPaymentDate > endPaymentDate)
            {
                _notification.AddErrorToastMessage("Data de inicio não pode ser maior que a data de término.");
                return RedirectToAction(nameof(Index));
            }

            var reportTithePayers = await _tithePayer.GetReportTithePayers(paymentType, name, startPaymentDate, endPaymentDate);

            return View(ROUTE_SCREEN_REPORTS, reportTithePayers);
        }

        public async Task<IActionResult> SearchTithes(int tithePayerCode, string document)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

                if (tithePayerCode == 0 && string.IsNullOrWhiteSpace(document))
                {
                    _notification.AddErrorToastMessage("Informe o documento ou código do dizimista.");
                    return RedirectToAction(nameof(ReportTithePayerPerTithe));
                }

                if (document != null)
                    document = document.Replace(".", "").Replace("-", "");

                List<TithePayerLaunchDTO> tithePayers = await _titheService.GetTithesWithFilters(null, tithePayerCode, document);

                if (tithePayers == null || tithePayers.Count == 0)
                {
                    _notification.AddErrorToastMessage("Dizimista não encontrado.");
                    return RedirectToAction(nameof(ReportTithePayerPerTithe));
                }

                List<TitheDTO> tithes = await _titheService.GetTithesByTithePayerId(tithePayers.First().TithePayerId);

                List<TitheDTO> tithesOrganized = tithes.OrderByDescending(t => t.PaymentMonth).ToList();

                return View(ROUTE_SCREEN_TITHE_PER_TITHEPAYER, tithesOrganized);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(ReportTithePayerPerTithe));

        }

        public async Task<IActionResult> SearchReportBirthdays(string name, DateTime startBirthdayDate, DateTime endBirthdayDate)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");

            if (startBirthdayDate == DateTime.MinValue)
            {
                startBirthdayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if (endBirthdayDate == DateTime.MinValue)
            {
                endBirthdayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            }

            if (startBirthdayDate > endBirthdayDate)
            {
                _notification.AddErrorToastMessage("Data de inicio não pode ser maior que a data de término.");
                return RedirectToAction(nameof(Index));
            }

            var reportTithePayers = await _tithePayer.GetReportTithePayersBirthdays(name, startBirthdayDate, endBirthdayDate);

            return View(ROUTE_SCREEN_BIRTHDAYS, reportTithePayers);
        }

    }
}
