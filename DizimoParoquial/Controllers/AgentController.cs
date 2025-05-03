using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;
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

        public AgentController(
            IToastNotification notification, 
            AgentService agentService,
            EventService eventService)
        {
            _notification = notification;
            _agentService = agentService;
            _eventService = eventService;
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

                process = "ACESSO TELA AGENTES";
                           
                details = $"{username} acessou tela de agentes!";

                eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                return View();
            }
            else
            {
                _notification.AddErrorToastMessage("Sessão encerrada, conecte-se novamente!");
                return RedirectToAction("Index","Login");
            }

        }

        public async Task<IActionResult> SearchAgents(string status, string name)
        {

            string? process, details, username;
            bool eventRegistered;

            int? idUser = HttpContext.Session.GetInt32("User");

            if(idUser != null && idUser != 0)
            {
                List<AgentDTO> agents = new List<AgentDTO>();

                username = HttpContext.Session.GetString("Username");

                ViewBag.UserName = username;

                try
                {

                    bool? statusConverted = status != null ? Convert.ToBoolean(status) : null;

                    if (status == null && name == null)
                        agents = await _agentService.GetAgentsWithouthFilters();

                    else
                        agents = await _agentService.GetAgentsWithFilters(statusConverted, name);

                    process = "PESQUISA DE AGENTES";

                    details = $"{username} pesquisou os agentes!";

                    eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "PESQUISA DE AGENTES";

                    details = $"{username} falhou na pesquisa de agentes. Erro: {ex.Message}";

                    eventRegistered = await _eventService.SaveEvent(process, details);

                }

                return View(ROUTE_SCREEN_AGENTS, agents);
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

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                        _notification.AddErrorToastMessage("Não foi possível criar o agente do dizimo!");

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "CRIAÇÃO DE AGENTES";

                    details = $"{username} falhou na criação de agentes. Erro: {ex.Message}";

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

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                        _notification.AddErrorToastMessage("Não foi possível editar o agente do dizimo!");
                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EDIÇÃO DE AGENTES";

                    details = $"{username} falhou na edição de agentes. Erro: {ex.Message}";

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

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }
                    else
                    {
                        _notification.AddErrorToastMessage("Erro ao excluir o agente!");
                        
                        process = "EXCLUSÃO DE AGENTES";

                        details = $"{username} não excluiu o agente: {agentId}, devido a erro gerado!";

                        eventRegistered = await _eventService.SaveEvent(process, details, userId: idUser);

                    }

                }
                catch (Exception ex)
                {
                    _notification.AddErrorToastMessage(ex.Message);

                    process = "EXCLUSÃO DE AGENTES";

                    details = $"{username} falhou na exclusão de agentes. Erro: {ex.Message}";

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
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
