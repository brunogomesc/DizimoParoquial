using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;

namespace DizimoParoquial.Data.Repositories
{
    public class AgentRepository : IAgentRepository
    {

        private readonly ConfigurationService _configurationService;

        public AgentRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<List<string>> GetAgentCodes()
        {
            List<string> existentCodes = new List<string>();

            try
            {
                var query = "SELECT AgentCode FROM Agent WHERE Deleted = false;";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<string>(query);

                    existentCodes = result.ToList();

                    return existentCodes;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar códigos agentes - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consultar códigos agentes - Erro interno.", ex);
            }
        }

        public async Task<bool> RegisterAgent(Agent agent)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"INSERT INTO Agent(Name, Active, AgentCode, CreatedAt, UpdatedAt, PhoneNumber) 
                                    VALUES(@Name, @Active, @AgentCode, @CreatedAt, @UpdatedAt, @PhoneNumber);";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                agent.Name,
                                agent.Active,
                                agent.AgentCode,
                                agent.CreatedAt,
                                agent.UpdatedAt,
                                agent.PhoneNumber
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return result > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Agente do Dizimo - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Agente do Dizimo - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<AgentDTO> GetAgentById(int id)
        {

            AgentDTO user = new AgentDTO();

            try
            {
                var query = "SELECT * FROM Agent WHERE AgentId = @AgentId";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<AgentDTO>(query,
                        new
                        {
                            AgentId = id
                        }
                    );

                    user = result.FirstOrDefault();

                    return user;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Agente - Erro interno.", ex);
            }
        }

        public async Task<AgentDTO> GetAgentByCode(string agentCode)
        {

            AgentDTO user = new AgentDTO();

            try
            {
                var query = "SELECT * FROM Agent WHERE AgentCode = @AgentCode";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<AgentDTO>(query,
                        new
                        {
                            AgentCode = agentCode
                        }
                    );

                    user = result.FirstOrDefault();

                    return user;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Agente - Erro interno.", ex);
            }
        }

        public async Task<bool> DeleteAgent(int agentId)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"UPDATE Agent 
                                    SET Deleted = @Deleted, 
                                        UpdatedAt = @UpdatedAt 
                                    WHERE AgentId = @AgentId";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                Deleted = true,
                                UpdatedAt = DateTime.Now,
                                AgentId = agentId
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return result > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Deletar Usuário - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Deletar Usuário - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<bool> UpdateAgent(Agent agent)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"UPDATE Agent 
                                    SET Name = @Name, 
                                        PhoneNumber = @PhoneNumber,
                                        Active = @Active, 
                                        UpdatedAt = @UpdatedAt 
                                    WHERE AgentId = @AgentId";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                agent.Name,
                                agent.PhoneNumber,
                                agent.Active,
                                agent.UpdatedAt,
                                agent.AgentId
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return result > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Agente - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Agente - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<List<AgentDTO>> GetAgentsWithouthFilters()
        {

            List<AgentDTO> agents = new List<AgentDTO>();

            try
            {
                var query = "SELECT * FROM Agent WHERE Deleted = false;";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<AgentDTO>(query);

                    agents = result.ToList();

                    return agents;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Agente - Erro interno.", ex);
            }
        }

        public async Task<List<AgentDTO>> GetAgentsWithFilters(bool? status, string? name)
        {

            List<AgentDTO> agents = new List<AgentDTO>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT * FROM Agent WHERE Deleted = false ");

                if (status != null)
                    query.Append($" AND Active = {status}");

                if (name != null)
                    query.Append($" AND Name LIKE '%{name}%' ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<AgentDTO>(query.ToString());

                    agents = result.ToList();

                    return agents;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Agente - Erro interno.", ex);
            }
        }

    }
}
