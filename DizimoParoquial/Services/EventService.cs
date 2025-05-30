using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using System.Data.Common;

namespace DizimoParoquial.Services
{
    public class EventService
    {

        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<bool> SaveEvent(string process, string details, int? userId = null, int? agentId = null)
        {
            bool eventWasCreated = false;

            try
            {

                Event newEvent = new Event
                {
                    EventDate = DateTime.Now,
                    Process = process,
                    Details = details,
                    UserId = userId,
                    AgentId = agentId
                };

                eventWasCreated = await SaveEventRepository(newEvent);

                return eventWasCreated;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar Evento - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar Evento - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar Evento - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar Evento - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar Evento - Dados vazios.");
            }
        }

        public async Task<List<ReportEvent>> GetReportEvents(string? agentName, DateTime startEventDate, DateTime endEventDate)
        {

            List<ReportEvent> reportEvents = new List<ReportEvent>();

            try
            {

                reportEvents = await GetReportEventsRepository(agentName, startEventDate, endEventDate);

                return reportEvents;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Obter Eventos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Obter Eventos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Obter Eventos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Obter Eventos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Obter Eventos - Dados vazios.");
            }
        }

        #region Repositories Methods

        private async Task<bool> SaveEventRepository(Event newEvent)
        {
            bool eventWasCreated = false;

            try
            {
                eventWasCreated = await _eventRepository.SaveEvent(newEvent);

                return eventWasCreated;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar Evento - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar Evento - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar Evento - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar Evento - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar Evento - Dados vazios.");
            }
        }

        public async Task<List<ReportEvent>> GetReportEventsRepository(string? agentName, DateTime startEventDate, DateTime endEventDate)
        {

            List<ReportEvent> reportEvents = new List<ReportEvent>();

            try
            {

                reportEvents = await _eventRepository.GetReportEvents(agentName, startEventDate, endEventDate);

                return reportEvents;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Obter Eventos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Obter Eventos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Obter Eventos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Obter Eventos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Obter Eventos - Dados vazios.");
            }
        }

        #endregion

    }
}
