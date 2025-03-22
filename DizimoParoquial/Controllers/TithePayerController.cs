using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils;
using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Threading.Tasks;

namespace DizimoParoquial.Controllers
{
    //[SessionAuthorize("MANTITPAY")]
    public class TithePayerController : Controller
    {

        private const string ROUTE_SCREEN_TITHEPAYERS = "/Views/TithePayer/Index.cshtml";
        private readonly IToastNotification _notification;
        private readonly TithePayerService _tithePayerService;

        public TithePayerController(IToastNotification notification, TithePayerService tithePayerService)
        {
            _notification = notification;
            _tithePayerService = tithePayerService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");

            List<TithePayer> tithePayers = await _tithePayerService.GetTithePayersWithouthFilters();

            int pageSize = 10;

            var paginatedTithePayers = PaginatedList<TithePayer>.CreateAsync(tithePayers.AsQueryable(), pageNumber, pageSize);

            return View(paginatedTithePayers);
        }

        public async Task<IActionResult> SearchTithePayer(string name, string document, int pageNumber = 1)
        {
            List<TithePayer> tithePayers = new List<TithePayer>();
            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

                if (document == null && name == null)
                    tithePayers = await _tithePayerService.GetTithePayersWithouthFilters();

                else
                    tithePayers = await _tithePayerService.GetTithePayersWithFilters(document?.Replace(".", "").Replace("-", ""), name);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            int pageSize = 10;

            var paginatedTithePayers = PaginatedList<TithePayer>.CreateAsync(tithePayers.AsQueryable(), pageNumber, pageSize);

            return View(ROUTE_SCREEN_TITHEPAYERS, paginatedTithePayers);
        }

        public async Task<IActionResult> SaveTithePayer(TithePayerDTO tithePayer)
        {

            try
            {
                ViewBag.UserName = HttpContext.Session.GetString("Username");

                if (string.IsNullOrWhiteSpace(tithePayer.Name)
                    || string.IsNullOrWhiteSpace(tithePayer.Document)
                    || tithePayer.DateBirth != DateTime.MinValue
                    || string.IsNullOrWhiteSpace(tithePayer.PhoneNumber))
                {
                    _notification.AddErrorToastMessage("Nome, Documento, Data de Nascimento e Telefone são obrigatórios!");
                    return RedirectToAction(nameof(Index));
                }

                int tithePayerCode = await _tithePayerService.RegisterTithePayer(tithePayer, Convert.ToInt32(HttpContext.Session.GetInt32("User")));

                _notification.AddSuccessToastMessage($"Dizimista {tithePayer.Name} cadastrado com sucesso! Código: {tithePayerCode}");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditTithePayer(TithePayerDTO tithePayer)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {
                if (tithePayer.Name == null)
                {
                    _notification.AddWarningToastMessage("Nome do dizimista não pode ser nulo.");
                    return RedirectToAction(nameof(Index));
                }

                bool tithePayerWasEdited = await _tithePayerService.UpdateTithePayer(tithePayer);

                if (tithePayerWasEdited)
                    _notification.AddSuccessToastMessage("Dizimista editado com sucesso!");
                else
                    _notification.AddErrorToastMessage("Não foi possível editar dizimista!");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetDetails(int id)
        {

            try
            {
                TithePayer tithePayer = await _tithePayerService.GetTithePayerById(id);

                if (tithePayer == null)
                {
                    _notification.AddErrorToastMessage("Agente do dizimo não localizado!");
                    return RedirectToAction(nameof(Index));
                }

                if (tithePayer.TermFile != null)
                {
                    string imgBase64 = Convert.ToBase64String(tithePayer.TermFile);
                    string urlImagem = $"data:image/jpeg;base64,{imgBase64}";
                    tithePayer.TermFile64Base = urlImagem;
                }

                return Json(tithePayer);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
