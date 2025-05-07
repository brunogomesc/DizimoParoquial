using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OfficeOpenXml;

namespace DizimoParoquial.Controllers
{
    public class ReportController : Controller
    {

        private const string ROUTE_SCREEN_REPORTS = "/Views/Report/ReportTithePayer.cshtml";
        private const string ROUTE_SCREEN_TITHE_PER_TITHEPAYER = "/Views/Report/ReportTithePayerPerTithe.cshtml";
        private const string ROUTE_SCREEN_BIRTHDAYS = "/Views/Report/ReportBirthdays.cshtml";
        private const string ROUTE_SCREEN_TITHES = "/Views/Report/ReportTithes.cshtml";

        private readonly IToastNotification _notification;
        private readonly TithePayerService _tithePayer;
        private readonly TitheService _titheService;
        private readonly EventService _eventService;

        public ReportController(
            IToastNotification notification, 
            TithePayerService tithePayer,
            TitheService titheService,
            EventService eventService)
        {
            _notification = notification;
            _tithePayer = tithePayer;
            _titheService = titheService;
            _eventService = eventService;
        }

        #region Views

        public IActionResult ReportValuePerPaymentType()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        public async Task<IActionResult> ReportTithePayer()
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {

                ViewBag.StartPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ViewBag.EndPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                
                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO RELATÓRIO ENTRADAS";

                details = $"{username} acessou tela de relatório de entradas!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> ReportTithePayerPerTithe()
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if (idUser != null && idUser != 0)
            {

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO RELATÓRIO DIZIMO POR DIZIMISTAS";

                details = $"{username} acessou tela de relatório de dizimo por dizimistas!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> ReportBirthdays()
        {
            
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if (idUser != null && idUser != 0)
            {

                ViewBag.StartBirthdayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ViewBag.EndBirthdayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO RELATÓRIO ANIVERSARIANTES";

                details = $"{username} acessou tela de relatório de aniversariantes!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> ReportTithes()
        {
            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if (idUser != null && idUser != 0)
            {

                ViewBag.StartTitheDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ViewBag.EndTitheDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                process = "ACESSO RELATÓRIO DIZIMOS";

                details = $"{username} acessou tela de relatório de dizimos!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        #endregion

        public async Task<IActionResult> SearchReportTithePayer(string paymentType, string name, DateTime startPaymentDate, DateTime endPaymentDate, bool generateExcel, string pageAmount, string page, string buttonPage)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {

                if (startPaymentDate == DateTime.MinValue)
                {
                    startPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }

                if (endPaymentDate == DateTime.MinValue)
                {
                    endPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                }

                if (startPaymentDate > endPaymentDate)
                {
                    _notification.AddErrorToastMessage("Data de inicio não pode ser maior que a data de término.");
                    return RedirectToAction(nameof(ReportTithePayer));
                }

                var reportTithePayers = await _tithePayer.GetReportTithePayers(paymentType, name, startPaymentDate, endPaymentDate);

                ViewBag.PaymentType = paymentType;
                ViewBag.Name = name;
                ViewBag.StartPaymentDate = startPaymentDate;
                ViewBag.EndPaymentDate = endPaymentDate;

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                #region Paginação

                int actualPage = 0;
                List<ReportTithePayer> tithePayersPaginated = new();

                if (buttonPage != null)
                {
                    actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                }

                int pageSize = pageAmount != null ? Convert.ToInt32(pageAmount) : 10;
                int count = 0;
                string action = page is null ? "" : page.Substring(3, page.Length - 3);
                int totalPages = reportTithePayers.Count % pageSize == 0 ? reportTithePayers.Count / pageSize : (reportTithePayers.Count / pageSize) + 1;
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

                if (reportTithePayers.Count > pageSize)
                {
                    for (int i = (actualPage * pageSize); i < reportTithePayers.Count; i++)
                    {
                        tithePayersPaginated.Add(reportTithePayers[i]);
                        count++;

                        if (count == pageSize)
                            break;
                    }
                }
                else
                {
                    tithePayersPaginated = reportTithePayers;
                }

                TempData["TotalCredenciais"] = reportTithePayers.Count;
                TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                TempData["UltimoRegistro"] = reportTithePayers.Count <= pageSize ? reportTithePayers.Count : (actualPage * pageSize) + count;

                #endregion

                process = "CONSULTA RELATÓRIO ENTRADAS";

                details = $"{username} consultou o relatório de entradas!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                if (generateExcel)
                {

                    process = "EXPORTAÇÃO RELATÓRIO ENTRADAS";

                    details = $"{username} exportou o relatório de entradas!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    return GenerateExcelTithePayers(reportTithePayers);
                }

                return View(ROUTE_SCREEN_REPORTS, tithePayersPaginated);
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        public async Task<IActionResult> SearchTithes(int tithePayerCode, string document, bool generateExcel, string pageAmount, string page, string buttonPage)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

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

                    #region Paginação

                    int actualPage = 0;
                    List<TitheDTO> tithePaginated = new();

                    if (buttonPage != null)
                    {
                        actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                    }

                    int pageSize = pageAmount != null ? Convert.ToInt32(pageAmount) : 10;
                    int count = 0;
                    string action = page is null ? "" : page.Substring(3, page.Length - 3);
                    int totalPages = tithes.Count % pageSize == 0 ? tithes.Count / pageSize : (tithes.Count / pageSize) + 1;
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

                    if (tithes.Count > pageSize)
                    {
                        for (int i = (actualPage * pageSize); i < tithes.Count; i++)
                        {
                            tithePaginated.Add(tithes[i]);
                            count++;

                            if (count == pageSize)
                                break;
                        }
                    }
                    else
                    {
                        tithePaginated = tithes;
                    }

                    TempData["TotalCredenciais"] = tithes.Count;
                    TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                    TempData["UltimoRegistro"] = tithes.Count <= pageSize ? tithes.Count : (actualPage * pageSize) + count;

                    #endregion

                    var tithesOrganized = tithes.OrderByDescending(t => t.PaymentMonth).ToList();

                    ViewBag.UserName = username;

                    process = "CONSULTA RELATÓRIO DIZIMOS";

                    details = $"{username} consultou o relatório de dizimos!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    if (generateExcel)
                    {

                        process = "EXPORTAÇÃO RELATÓRIO DIZIMOS";

                        details = $"{username} exportou o relatório de dizimos!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                        return GenerateExcelTithes(tithesOrganized);
                    }

                    return View(ROUTE_SCREEN_TITHE_PER_TITHEPAYER, tithePaginated.OrderByDescending(t => t.PaymentMonth).ToList());

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "CONSULTA RELATÓRIO DE DIZIMOS";

                    details = $"{username} falhou na consulta do relatório de dizimo. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(ReportTithePayerPerTithe));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        public async Task<IActionResult> SearchReportBirthdays(string name, DateTime startBirthdayDate, DateTime endBirthdayDate, bool generateExcel, string pageAmount, string page, string buttonPage)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

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
                    return RedirectToAction(nameof(ReportBirthdays));
                }

                var birthdays = await _tithePayer.GetReportTithePayersBirthdays(name, startBirthdayDate, endBirthdayDate);

                ViewBag.Name = name;
                ViewBag.StartBirthdayDate = startBirthdayDate;
                ViewBag.EndBirthdayDate = endBirthdayDate;

                List<ReportBirthday>? reportBirthdays = new List<ReportBirthday>();

                if (birthdays != null)
                    reportBirthdays = birthdays.OrderBy(b => b.DateBirth).ToList();

                #region Paginação

                int actualPage = 0;
                List<ReportBirthday> birthdaysPaginated = new();

                if (buttonPage != null)
                {
                    actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                }

                int pageSize = pageAmount != null ? Convert.ToInt32(pageAmount) : 10;
                int count = 0;
                string action = page is null ? "" : page.Substring(3, page.Length - 3);
                int totalPages = reportBirthdays.Count % pageSize == 0 ? reportBirthdays.Count / pageSize : (reportBirthdays.Count / pageSize) + 1;
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

                if (reportBirthdays.Count > pageSize)
                {
                    for (int i = (actualPage * pageSize); i < reportBirthdays.Count; i++)
                    {
                        birthdaysPaginated.Add(reportBirthdays[i]);
                        count++;

                        if (count == pageSize)
                            break;
                    }
                }
                else
                {
                    birthdaysPaginated = reportBirthdays;
                }

                TempData["TotalCredenciais"] = reportBirthdays.Count;
                TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                TempData["UltimoRegistro"] = reportBirthdays.Count <= pageSize ? reportBirthdays.Count : (actualPage * pageSize) + count;

                #endregion

                ViewBag.UserName = username;

                process = "CONSULTA RELATÓRIO ANIVERSARIANTES";

                details = $"{username} consultou o relatório de aniversariantes!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                if (generateExcel)
                {

                    process = "EXPORTAÇÃO RELATÓRIO ANIVERSARIANTES";

                    details = $"{username} exportou o relatório de aniversariantes!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    return GenerateExcelBirthdays(reportBirthdays);
                }

                return View(ROUTE_SCREEN_BIRTHDAYS, birthdaysPaginated);
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        public async Task<IActionResult> SearchReportTithesMonth(string paymentType, string name, DateTime startPaymentDate, DateTime endPaymentDate, bool generateExcel, string pageAmount, string page, string buttonPage)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            username = HttpContext.Session.GetString("Username");

            if (idUser != null && idUser != 0)
            {

                if (startPaymentDate == DateTime.MinValue)
                {
                    startPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }

                if (endPaymentDate == DateTime.MinValue)
                {
                    endPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                }

                if (startPaymentDate > endPaymentDate)
                {
                    _notification.AddErrorToastMessage("Data de inicio não pode ser maior que a data de término.");
                    return RedirectToAction(nameof(ReportTithes));
                }

                var reportTithes = await _titheService.GetReportTithesMonth(paymentType, name, startPaymentDate, endPaymentDate);

                ViewBag.PaymentType = paymentType;
                ViewBag.Name = name;
                ViewBag.StartTitheDate = startPaymentDate;
                ViewBag.EndTitheDate = endPaymentDate;

                #region Paginação

                int actualPage = 0;
                List<TitheDTO> tithesPaginated = new();

                if (buttonPage != null)
                {
                    actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                }

                int pageSize = pageAmount != null ? Convert.ToInt32(pageAmount) : 10;
                int count = 0;
                string action = page is null ? "" : page.Substring(3, page.Length - 3);
                int totalPages = reportTithes.Count % pageSize == 0 ? reportTithes.Count / pageSize : (reportTithes.Count / pageSize) + 1;
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

                if (reportTithes.Count > pageSize)
                {
                    for (int i = (actualPage * pageSize); i < reportTithes.Count; i++)
                    {
                        tithesPaginated.Add(reportTithes[i]);
                        count++;

                        if (count == pageSize)
                            break;
                    }
                }
                else
                {
                    tithesPaginated = reportTithes;
                }

                TempData["TotalCredenciais"] = reportTithes.Count;
                TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                TempData["UltimoRegistro"] = reportTithes.Count <= pageSize ? reportTithes.Count : (actualPage * pageSize) + count;

                #endregion

                ViewBag.UserName = username;

                process = "CONSULTA RELATÓRIO DIZIMOS";

                details = $"{username} consultou o relatório de dizimos!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                if (generateExcel)
                {

                    process = "EXPORTAÇÃO RELATÓRIO DIZIMOS";

                    details = $"{username} exportou o relatório de dizimos!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    return GenerateExcelTithes(reportTithes);
                }

                return View(ROUTE_SCREEN_TITHES, tithesPaginated);
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }


        #region Excel

        [HttpPost]
        public IActionResult GenerateExcelBirthdays(List<ReportBirthday> birthdays)
        {

            if (birthdays == null || birthdays.Count == 0)
            {
                _notification.AddErrorToastMessage("Sem aniversariantes para exportar!");
                return View(ROUTE_SCREEN_BIRTHDAYS);
            }

            // Configuração para permitir a geração do arquivo
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // Criar uma nova planilha
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Aniversariantes");

                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "Código Dizimista";
                worksheet.Cells[1, 2].Value = "Nome Dizimista";
                worksheet.Cells[1, 3].Value = "Documento";
                worksheet.Cells[1, 4].Value = "Date de Aniversário";
                worksheet.Cells[1, 5].Value = "Telefone";
                worksheet.Cells[1, 6].Value = "E-mail";

                // Preencher os dados a partir da lista de objetos
                int row = 2;

                foreach (var birthday in birthdays)
                {
                    worksheet.Cells[row, 1].Value = birthday.TithePayerId;
                    worksheet.Cells[row, 2].Value = birthday.Name;
                    worksheet.Cells[row, 3].Value = birthday.Document;
                    worksheet.Cells[row, 4].Value = birthday.DateBirth.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 5].Value = birthday.PhoneNumber;
                    worksheet.Cells[row, 6].Value = birthday.Email;

                    row++;
                }

                // Formatar cabeçalhos
                worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 6].AutoFitColumns();

                var excelBytes = package.GetAsByteArray();

                // Retornar o arquivo para download
                string excelFileName = $"Aniversariantes_{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Replace("/", "").Replace(" ", "").Replace(":", "")}.xlsx";

               
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
            }

        }

        [HttpPost]
        public IActionResult GenerateExcelTithes(List<TitheDTO> tithes)
        {

            if (tithes == null || tithes.Count == 0)
            {
                _notification.AddErrorToastMessage("Sem dizimos para exportar!");
                return View(ROUTE_SCREEN_TITHE_PER_TITHEPAYER);
            }

            // Configuração para permitir a geração do arquivo
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // Criar uma nova planilha
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Dizimo");

                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "Nome Dizimista";
                worksheet.Cells[1, 2].Value = "Nome Agente";
                worksheet.Cells[1, 3].Value = "Forma de Contribuição";
                worksheet.Cells[1, 4].Value = "Valor";
                worksheet.Cells[1, 5].Value = "Competência";
                worksheet.Cells[1, 6].Value = "Data de Contribuição";

                // Preencher os dados a partir da lista de objetos
                int row = 2;

                foreach (var tithe in tithes)
                {
                    worksheet.Cells[row, 1].Value = tithe.NameTithePayer;
                    worksheet.Cells[row, 2].Value = tithe.NameAgent;
                    worksheet.Cells[row, 3].Value = tithe.PaymentType;
                    worksheet.Cells[row, 4].Value = $"R$ {tithe.Value.ToString("F2")}";
                    worksheet.Cells[row, 5].Value = tithe.PaymentMonth.ToString("MM/yyyy");
                    worksheet.Cells[row, 6].Value = tithe.RegistrationDate.ToString("dd/MM/yyyy");

                    row++;
                }

                // Formatar cabeçalhos
                worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 6].AutoFitColumns();

                var excelBytes = package.GetAsByteArray();

                string tithePayerName = tithes.First().NameTithePayer.Split(' ').First();

                // Retornar o arquivo para download
                string excelFileName = $"Dizimos_{tithePayerName}_{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Replace("/", "").Replace(" ", "").Replace(":", "")}.xlsx";


                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
            }

        }

        [HttpPost]
        public IActionResult GenerateExcelTithePayers(List<ReportTithePayer> tithePayers)
        {

            if (tithePayers == null || tithePayers.Count == 0)
            {
                _notification.AddErrorToastMessage("Sem dizimistas para exportar!");
                return View(ROUTE_SCREEN_TITHE_PER_TITHEPAYER);
            }

            // Configuração para permitir a geração do arquivo
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // Criar uma nova planilha
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Entradas");

                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "Nome Dizimista";
                worksheet.Cells[1, 2].Value = "Valor";
                worksheet.Cells[1, 3].Value = "Data de Contribuição";
                worksheet.Cells[1, 4].Value = "Forma de Contribuição";

                // Preencher os dados a partir da lista de objetos
                int row = 2;

                foreach (var tithePayer in tithePayers)
                {
                    worksheet.Cells[row, 1].Value = tithePayer.Name;
                    worksheet.Cells[row, 2].Value = $"R$ {tithePayer.Value.ToString("F2")}";
                    worksheet.Cells[row, 3].Value = tithePayer.PaymentDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = tithePayer.PaymentType;

                    row++;
                }

                // Formatar cabeçalhos
                worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 4].AutoFitColumns();

                var excelBytes = package.GetAsByteArray();

                // Retornar o arquivo para download
                string excelFileName = $"Entradas_{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Replace("/", "").Replace(" ", "").Replace(":", "")}.xlsx";

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
            }

        }

        #endregion

    }
}
