using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    public class ReportController : Controller
    {

        private const string ROUTE_SCREEN_REPORTS = "/Views/Report/ReportTithePayer.cshtml";
        private readonly IToastNotification _notification;
        private readonly TithePayerService _tithePayer;


        public ReportController(IToastNotification notification, TithePayerService tithePayer)
        {
            _notification = notification;
            _tithePayer = tithePayer;
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

    }
}
