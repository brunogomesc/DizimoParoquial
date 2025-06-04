using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils;
using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using NToastNotify;
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace DizimoParoquial.Controllers
{
    //[SessionAuthorize("MANAGENT")]
    public class AgentController : Controller
    {

        private const string ROUTE_SCREEN_AGENTS = "/Views/Agent/Index.cshtml";
        private readonly IToastNotification _notification;
        private readonly AgentService _agentService;
        private readonly EventService _eventService;
        private readonly ILogger<AgentController> _log;

        public AgentController(
            IToastNotification notification, 
            AgentService agentService,
            EventService eventService,
            ILogger<AgentController> log)
        {
            _notification = notification;
            _agentService = agentService;
            _eventService = eventService;
            _log = log;
        }

        public async Task<IActionResult> Index()
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;
                ViewBag.Status = null;
                ViewBag.Name = null;

                process = "ACESSO TELA AGENTES";
                           
                details = $"{username} acessou tela de agentes!";

                _log.LogInformation(details);

                //eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index","Login");
            }

        }

        public async Task<IActionResult> SearchAgents(string status, string name, string amountPages, string page, string buttonPage)
        {

            string? process, details, username;
            bool eventRegistered;
            List<AgentDTO> agentsPaginated = new();

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                List<AgentDTO> agents = new List<AgentDTO>();

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;
                ViewBag.Status = status;
                ViewBag.Name = name;

                try
                {

                    bool? statusConverted = status != null ? Convert.ToBoolean(status) : null;

                    if (status == null && name == null)
                        agents = await _agentService.GetAgentsWithouthFilters();

                    else
                        agents = await _agentService.GetAgentsWithFilters(statusConverted, name);

                    #region Paginação

                    int actualPage = 0;

                    if (buttonPage != null)
                    {
                        actualPage = Convert.ToInt32(buttonPage.Substring(0, (buttonPage.IndexOf("_")))) - 1;
                    }
                    else if (page != null)
                    {
                        actualPage = Convert.ToInt32(page.Substring(0, (page.IndexOf("_"))));
                    }

                    int pageSize = amountPages != null ? Convert.ToInt32(amountPages) : 10;
                    int count = 0;
                    string action = page is null ? "" : page.Substring(3, page.Length - 3);
                    int totalPages = agents.Count % pageSize == 0 ? agents.Count / pageSize : (agents.Count / pageSize) + 1;
                    ViewBag.TotalPages = totalPages;
                    ViewBag.ActualPage = actualPage;

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

                    if (agents.Count > pageSize)
                    {
                        for (int i = (actualPage * pageSize); i < agents.Count; i++)
                        {
                            agentsPaginated.Add(agents[i]);
                            count++;

                            if (count == pageSize)
                                break;
                        }
                    }
                    else
                    {
                        agentsPaginated = agents;
                    }

                    TempData["TotalCredenciais"] = agents.Count;
                    TempData["PrimeiroRegistro"] = (actualPage * pageSize) + 1;
                    TempData["UltimoRegistro"] = agents.Count <= pageSize ? agents.Count : (actualPage * pageSize) + count;

                    #endregion

                    ViewBag.AmountPages = AmountPages.GetAmountPageInput();

                    process = "PESQUISA DE AGENTES";

                    details = $"{username} pesquisou os agentes!";

                    _log.LogInformation(details, agentsPaginated);

                    //eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "PESQUISA DE AGENTES";

                    details = $"{username} falhou na pesquisa de agentes. Erro: {ex.Message}";

                    _log.LogError(ex, details, ex.Data);

                    //eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return View(ROUTE_SCREEN_AGENTS, agentsPaginated);
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> SaveAgent(string name, string phoneNumber)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                try
                {
                    if (name == null)
                    {
                        _notification.AddWarningToastMessage("Nome do agente do dizimo é obrigatório.");
                        return RedirectToAction(nameof(Index));
                    }

                    if (phoneNumber == null)
                    {
                        _notification.AddWarningToastMessage("Telefone do Agente do dizimo é obrigatório.");
                        return RedirectToAction(nameof(Index));
                    }

                    string newAgenteCode = await _agentService.RegisterAgent(name, phoneNumber);

                    if (!string.IsNullOrWhiteSpace(newAgenteCode))
                    {
                        _notification.AddSuccessToastMessage("Agente do dizimo criado com sucesso! <b>Código do Agente: </b>" + newAgenteCode);

                        process = "CRIAÇÃO DE AGENTES";

                        details = $"{username} criou o agente: {name}!";

                        _log.LogInformation(details, newAgenteCode);

                        //eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                        _notification.AddErrorToastMessage("Não foi possível criar o agente do dizimo!");

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "CRIAÇÃO DE AGENTES";

                    details = $"{username} falhou na criação de agentes. Erro: {ex.Message}";

                    _log.LogError(ex, details, ex.Data);

                    //eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }

        }

        public async Task<IActionResult> EditAgent(Agent agent)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                try
                {
                    if (agent.Name == null)
                    {
                        _notification.AddWarningToastMessage("Nome do agente do dizimo é obrigatório.");
                        return RedirectToAction(nameof(Index));
                    }

                    if (agent.PhoneNumber == null)
                    {
                        _notification.AddWarningToastMessage("Telefone do agente do dizimo é obrigatório.");
                        return RedirectToAction(nameof(Index));
                    }

                    bool agentWasEdited = await _agentService.UpdateAgent(agent);

                    if (agentWasEdited)
                    {
                        _notification.AddSuccessToastMessage("Agente do dizimo editado com sucesso!");

                        process = "EDIÇÃO DE AGENTES";

                        details = $"{username} editou o agente: {agent.Name}!";

                        _log.LogInformation(details, agent);

                        //eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                        _notification.AddErrorToastMessage("Não foi possível editar o agente do dizimo!");
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EDIÇÃO DE AGENTES";

                    details = $"{username} falhou na edição de agentes. Erro: {ex.Message}";

                    _log.LogError(ex, details, ex.Data);

                    //eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> DeleteAgent(int agentId)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if (idUser != null && idUser != 0)
            {

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                try
                {
                    if (agentId == 0)
                    {
                        _notification.AddErrorToastMessage("Dados do agente não localizado para a exclusão!");
                        return RedirectToAction(nameof(Index));
                    }

                    bool agentWasDeleted = await _agentService.DeleteAgent(agentId);

                    if (agentWasDeleted)
                    {
                        _notification.AddSuccessToastMessage("Agente excluido com sucesso!");

                        process = "EXCLUSÃO DE AGENTES";

                        details = $"{username} excluiu o agente: {agentId}!";

                        _log.LogInformation(details, agentId);

                        //eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Erro ao excluir o agente!");
                        
                        process = "EXCLUSÃO DE AGENTES";

                        details = $"{username} não excluiu o agente: {agentId}, devido a erro gerado!";

                        _log.LogError(details, agentId);

                        //eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EXCLUSÃO DE AGENTES";

                    details = $"{username} falhou na exclusão de agentes. Erro: {ex.Message}";

                    _log.LogError(ex, details, ex.Data);

                    //eventRegistered = await _eventService.SaveEvent(process, details);

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

            try
            {
                AgentDTO agent = await _agentService.GetAgentById(id);

                if (agent == null)
                {
                    _notification.AddErrorToastMessage("Agente do dizimo não localizado!");
                    return RedirectToAction(nameof(Index));
                }

                return Json(agent);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
                _log.LogError(ex, ex.Message, ex.Data);
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
