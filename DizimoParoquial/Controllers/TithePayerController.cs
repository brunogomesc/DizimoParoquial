using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System;
using System.Diagnostics;

namespace DizimoParoquial.Controllers
{
    //[SessionAuthorize("MANTITPAY")]
    public class TithePayerController : Controller
    {

        private const string ROUTE_SCREEN_TITHEPAYERS = "/Views/TithePayer/Index.cshtml";
        private readonly IToastNotification _notification;
        private readonly TithePayerService _tithePayerService;
        private readonly EventService _eventService;

        public TithePayerController(
            IToastNotification notification, 
            TithePayerService tithePayerService,
            EventService eventService)
        {
            _notification = notification;
            _tithePayerService = tithePayerService;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(string pageAmount, string page, string buttonPage)
        {
            int actualPage = 0;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {

                List<TithePayer> tithePayers = await _tithePayerService.GetTithePayersWithouthFilters();

                #region Paginação

                int pageSize = pageAmount != null ? Convert.ToInt32(pageAmount) : 10;
                int count = 0;
                string action = page is null ? "" : page.Substring(3, page.Length - 3);
                int totalPages = tithePayers.Count % pageSize == 0 ? tithePayers.Count / pageSize : (tithePayers.Count / pageSize) + 1;
                ViewBag.TotalPages = totalPages;
                ViewBag.ActualPage = 0;

                if (action.Contains("back") || action.Contains("next"))
                {
                    actualPage = action.Contains("back") ? ViewBag.ActualPage - 1 : ViewBag.ActualPage + 1;
                    ViewBag.ActualPage = actualPage;
                }
                else if (buttonPage != null)
                {
                    actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                    ViewBag.ActualPage = actualPage;
                }

                List<TithePayer> tithePayersPaginated = new();

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
                TempData["UltimoRegistro"] = (actualPage * pageSize) + count;

                #endregion

                #region Event

                string? process, details, username;
                bool eventRegistered;

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO TELA DIZIMISTAS";

                details = $"{username} acessou tela de dizimistas!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                #endregion

                return View(ROUTE_SCREEN_TITHEPAYERS, tithePayersPaginated);
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        public async Task<IActionResult> SearchTithePayer(string name, string document, string pageAmount, string page, string buttonPage)
        {
            string? process, details, username;
            bool eventRegistered;
            List<TithePayer> tithePayersPaginated = new();

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {
                List<TithePayer> tithePayers = new List<TithePayer>();

                try
                {

                    if (document == null && name == null)
                        tithePayers = await _tithePayerService.GetTithePayersWithouthFilters();

                    else
                        tithePayers = await _tithePayerService.GetTithePayersWithFilters(document?.Replace(".", "").Replace("-", ""), name);

                    #region Paginação

                    int actualPage = 0;

                    if (buttonPage != null)
                    {
                        actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                    }

                    int pageSize = pageAmount != null ? Convert.ToInt32(pageAmount) : 10;
                    int count = 0;
                    string action = page is null ? "" : page.Substring(3, page.Length - 3);
                    int totalPages = tithePayers.Count % pageSize == 0 ? tithePayers.Count / pageSize : (tithePayers.Count / pageSize) + 1;
                    ViewBag.TotalPages = totalPages;
                    ViewBag.ActualPage = 0;

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
                    TempData["UltimoRegistro"] = (actualPage * pageSize) + count;

                    #endregion

                    ViewBag.UserName = username;

                    process = "FILTRAGEM DIZIMISTAS";

                    details = $"{username} filtrou os dizimistas!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "FILTRAGEM DE DIZIMISTAS";

                    details = $"{username} falhou na filtragem de dizimistas. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return View(ROUTE_SCREEN_TITHEPAYERS, tithePayersPaginated);
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> SaveTithePayer(TithePayerDTO tithePayer)
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(tithePayer.Name)
                        || tithePayer.DateBirth == DateTime.MinValue)
                    {
                        _notification.AddErrorToastMessage("Nome e Data de Nascimento são obrigatórios!");
                        return RedirectToAction(nameof(Index));
                    }

                    int tithePayerCode = await _tithePayerService.RegisterTithePayer(tithePayer, Convert.ToInt32(HttpContext.Session.GetInt32("User")));

                    _notification.AddSuccessToastMessage($"Dizimista {tithePayer.Name} cadastrado com sucesso! Código: {tithePayerCode}");

                    ViewBag.UserName = username;

                    process = "CRIAÇÃO DIZIMISTA";

                    details = $"{username} criou o dizimista {tithePayer.Name} com sucesso!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "CRIAÇÃO DIZIMISTA";

                    details = $"{username} na criação do dizimista. Erro: {ex.Message}";

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

        public async Task<IActionResult> EditTithePayer(TithePayerDTO tithePayer)
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

                try
                {
                    if (tithePayer.Name == null)
                    {
                        _notification.AddWarningToastMessage("Nome do dizimista não pode ser nulo.");
                        return RedirectToAction(nameof(Index));
                    }

                    bool tithePayerWasEdited = await _tithePayerService.UpdateTithePayer(tithePayer);

                    ViewBag.UserName = username;

                    process = "EDIÇÃO DIZIMISTA";

                    if (tithePayerWasEdited)
                    {
                        _notification.AddSuccessToastMessage("Dizimista editado com sucesso!");

                        details = $"{username} editou o dizimista com sucesso!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Não foi possível editar dizimista!");

                        details = $"{username} não foi possível editar o dizimista, devido a uma falha!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                        
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EDIÇÃO DIZIMISTA";

                    details = $"{username} falhou na edição de dizimistas. Erro: {ex.Message}";

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

        public async Task<IActionResult> GetDetails(int id)
        {

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
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

                        string urlImagem = string.Empty;

                        switch (tithePayer.Extension)
                        {
                            case ".pdf":
                                urlImagem = $"data:application/pdf;base64,{imgBase64}";
                                break;

                            case ".png":
                                urlImagem = $"data:image/png;base64,{imgBase64}";
                                break;

                            case ".jpeg":
                            case ".jpg":
                                urlImagem = $"data:image/jpeg;base64,{imgBase64}";
                                break;
                        }

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
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

    }
}
