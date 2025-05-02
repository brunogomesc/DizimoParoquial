using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using MySql.Data.MySqlClient;
using System.Data.Common;

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

                        var rowsEffect = await connection.ExecuteAsync(query,
                            new
                            {
                                newEvent.EventDate,
                                newEvent.Process,
                                newEvent.Details,
                                newEvent.UserId,
                                newEvent.AgentId
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return rowsEffect > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Evento - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Evento - Erro interno.", ex);
                    }
                }
            }
        }
    }
}
