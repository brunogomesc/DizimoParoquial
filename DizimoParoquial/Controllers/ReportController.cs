using Microsoft.AspNetCore.Mvc;

namespace DizimoParoquial.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult ReportValuePerPaymentType()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }
    }
}
