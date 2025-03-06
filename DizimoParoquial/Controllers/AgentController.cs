using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using DizimoParoquial.Utils.Filters;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DizimoParoquial.Controllers
{
    [SessionAuthorize("MANAGENT")]
    public class AgentController : Controller
    {

        private const string ROUTE_SCREEN_AGENTS = "/Views/Agent/Index.cshtml";
        private readonly IToastNotification _notification;
        private readonly AgentService _agentService;

        public AgentController(IToastNotification notification, AgentService agentService)
        {
            _notification = notification;
            _agentService = agentService;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View();
        }

        public async Task<IActionResult> SearchAgents(string status, string name)
        {
            List<AgentDTO> agents = new List<AgentDTO>();

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {

                bool? statusConverted = status != null ? Convert.ToBoolean(status) : null;

                if (status == null && name == null)
                    agents = await _agentService.GetAgentsWithouthFilters();

                else
                    agents = await _agentService.GetAgentsWithFilters(statusConverted, name);

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return View(ROUTE_SCREEN_AGENTS, agents);
        }

        public async Task<IActionResult> SaveAgent(string name)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {
                if (name == null)
                {
                    _notification.AddWarningToastMessage("Nome do agente do dizimo não pode ser nulo.");
                    return RedirectToAction(nameof(Index));
                }

                string newAgenteCode = await _agentService.RegisterAgent(name);

                if (!string.IsNullOrWhiteSpace(newAgenteCode))
                    _notification.AddSuccessToastMessage("Agente do dizimo criado com sucesso! <b>Código do Agente: </b>" + newAgenteCode);
                else
                    _notification.AddErrorToastMessage("Não foi possível criar o agente do dizimo!");

            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> EditAgent(Agent agent)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {
                if (agent.Name == null)
                {
                    _notification.AddWarningToastMessage("Nome do agente do dizimo não pode ser nulo.");
                    return RedirectToAction(nameof(Index));
                }

                bool agentWasEdited = await _agentService.UpdateAgent(agent);

                if (agentWasEdited)
                    _notification.AddSuccessToastMessage("Agente do dizimo editado com sucesso!");
                else
                    _notification.AddErrorToastMessage("Não foi possível editar o agente do dizimo!");
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAgent(int agentId)
        {

            ViewBag.UserName = HttpContext.Session.GetString("Username");

            try
            {
                if (agentId == 0)
                {
                    _notification.AddErrorToastMessage("Dados do agente não localizado para a exclusão!");
                    return RedirectToAction(nameof(Index));
                }

                bool agentWasDeleted = await _agentService.DeleteAgent(agentId);

                if (agentWasDeleted)
                    _notification.AddSuccessToastMessage("Agente excluido com sucesso!");
                else
                    _notification.AddErrorToastMessage("Erro ao excluir o agente!");
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
