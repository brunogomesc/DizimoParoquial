using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;

namespace DizimoParoquial.Data.Repositories
{
    public class EventRepository : IEventRepository
    {

        private readonly ConfigurationService _configurationService;

        public EventRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<bool> SaveEvent(Event newEvent)
        {
            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"INSERT INTO Event(EventDate, Process, Details, UserId, AgentId) 
                              VALUES(@EventDate, @Process, @Details, @UserId, @AgentId);";

                        DateTime eventDateForDb;
                        if (newEvent.EventDate.Kind == DateTimeKind.Utc)
                        {
                            eventDateForDb = newEvent.EventDate;
                        }
                        else if (newEvent.EventDate.Kind == DateTimeKind.Unspecified)
                        {
                            eventDateForDb = DateTime.SpecifyKind(newEvent.EventDate, DateTimeKind.Local).ToUniversalTime();
                        }
                        else
                        {
                            eventDateForDb = newEvent.EventDate.ToUniversalTime();
                        }

                        var rowsEffect = await connection.ExecuteAsync(query,
                            new
                            {
                                EventDate = eventDateForDb, // Use a data convertida para UTC
                                newEvent.Process,
                                newEvent.Details,
                                newEvent.UserId,
                                newEvent.AgentId
                            }
                        );

                        transaction.Commit();
                        return rowsEffect > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        throw new RepositoryException("Criar Evento - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new RepositoryException("Criar Evento - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<List<ReportEvent>> GetReportEvents(string? agentName, DateTime startEventDate, DateTime endEventDate)
        {

            List<ReportEvent> events = new List<ReportEvent>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT E.EventId, E.EventDate, ");
                query.Append("E.Process, E.Details, ");
                query.Append("A.Name as NameAgent, U.Name as NameUser ");
                query.Append("FROM Event E ");
                query.Append("LEFT JOIN Agent A ");
                query.Append("ON E.AgentId = A.AgentId ");
                query.Append("LEFT JOIN User U ");
                query.Append("ON E.UserId = U.UserId ");

                query.Append($"WHERE E.EventDate BETWEEN '{startEventDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{endEventDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}' ");

                if (agentName != null)
                    query.Append($" AND A.Name LIKE '%{agentName}%' ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<ReportEvent>(query.ToString());

                    events = result.ToList();

                    return events;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Eventos - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Eventos - Erro interno.", ex);
            }
        }

    }
}
