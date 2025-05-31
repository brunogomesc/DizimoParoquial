using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Utils;
using System.Data.Common;

namespace DizimoParoquial.Services
{
    public class AgentService
    {

        private readonly IAgentRepository _agentRepository;

        public AgentService(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
        }

        public string GenerateAgentCode(List<string> existentCodes)
        {
            Random generate = new Random();
            string code;

            do
            {
                code = generate.Next(1000, 10000).ToString();
            }
            while (existentCodes.Any(c => c == code));

            return code;
        }

        public async Task<string> RegisterAgent(string name, string phoneNumber)
        {
            bool agentWasCreated = false;

            try
            {

                List<string> existentCodes = await GetAgentCodesRepository();

                string newAgentCode = GenerateAgentCode(existentCodes);

                Agent agent = new Agent
                {
                    Name = name,
                    PhoneNumber = phoneNumber,
                    AgentCode = newAgentCode,
                    Active = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null
                };

                agentWasCreated = await RegisterAgentRepository(agent);

                if(agentWasCreated)
                    return newAgentCode;
                else 
                    return string.Empty;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar Agente Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar Agente Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar Agente Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar Agente Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar Agente Dizimo - Dados vazios.");
            }
        }

        public async Task<AgentDTO> GetAgentById(int id)
        {
            AgentDTO agent = new AgentDTO();

            try
            {
                agent = await GetAgentByIdRepository(id);

                return agent;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Buscar Agente Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Buscar Agente Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Buscar Agente Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Buscar Agente Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Buscar Agente Dizimo - Dados vazios.");
            }
        }

        public async Task<AgentDTO> GetAgentByCode(string agentCode)
        {
            AgentDTO agent = new AgentDTO();

            try
            {
                agent = await GetAgentByCodeRepository(agentCode);

                return agent;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Buscar Agente Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Buscar Agente Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Buscar Agente Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Buscar Agente Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Buscar Agente Dizimo - Dados vazios.");
            }
        }

        public async Task<bool> UpdateAgent(Agent agent)
        {
            bool agentWasUpdated = false;

            try
            {

                AgentDTO agentExists = await GetAgentByIdRepository(agent.AgentId);

                if (agentExists == null || agentExists.AgentId == 0)
                    throw new ValidationException("Agente não existente");

                agent.UpdatedAt = DateTime.Now;

                agentWasUpdated = await UpdateAgentRepository(agent);

                return agentWasUpdated;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Atualizar Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Atualizar Agente - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Atualizar Agente - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Atualizar Agente - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Atualizar Agente - Dados vazios.");
            }
        }

        public async Task<bool> DeleteAgent(int agentId)
        {
            bool agentWasDeleted = false;

            try
            {
                agentWasDeleted = await DeleteAgentRepository(agentId);


                return agentWasDeleted;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Excluir Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Excluir Agente - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Excluir Agente - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Excluir Agente - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Excluir Agente - Dados vazios.");
            }
        }

        public async Task<List<AgentDTO>> GetAgentsWithouthFilters()
        {
            List<AgentDTO> agents = new List<AgentDTO>();

            try
            {

                agents = await GetAgentsWithouthFiltersRepository();

                return agents;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Agente - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Agente - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Agente - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Agente - Dados vazios.");
            }
        }

        public async Task<List<AgentDTO>> GetAgentsWithFilters(bool? status, string? name)
        {
            List<AgentDTO> agents = new List<AgentDTO>();

            try
            {

                agents = await GetAgentsWithFiltersRepository(status, name);

                return agents;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Agente - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Agente - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Agente - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Agente - Dados vazios.");
            }
        }

        #region Repositories Methods

        private async Task<List<string>> GetAgentCodesRepository()
        {
            List<string> existentCodes = new List<string>();

            try
            {
                existentCodes = await _agentRepository.GetAgentCodes();

                return existentCodes;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Listar Códigos Agentes - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Listar Códigos Agentes - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Listar Códigos Agentes - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Listar Códigos Agentes - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Listar Códigos Agentes - Dados vazios.");
            }
        }

        private async Task<AgentDTO> GetAgentByIdRepository(int id)
        {
            AgentDTO agent = new AgentDTO();

            try
            {
                agent = await _agentRepository.GetAgentById(id);

                return agent;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Agente do Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Agente do Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Agente do Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Agente do Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Agente do Dizimo - Dados vazios.");
            }
        }

        private async Task<AgentDTO> GetAgentByCodeRepository(string agentCode)
        {
            AgentDTO agent = new AgentDTO();

            try
            {
                agent = await _agentRepository.GetAgentByCode(agentCode);

                return agent;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Agente do Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Agente do Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Agente do Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Agente do Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Agente do Dizimo - Dados vazios.");
            }
        }

        private async Task<bool> RegisterAgentRepository(Agent agent)
        {
            bool agentWasCreated = false;

            try
            {
                agentWasCreated = await _agentRepository.RegisterAgent(agent);

                return agentWasCreated;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar Agente Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar Agente Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar Agente Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar Agente Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar Agente Dizimo - Dados vazios.");
            }
        }

        private async Task<bool> UpdateAgentRepository(Agent agent)
        {
            bool userWasUpdated = false;

            try
            {
                userWasUpdated = await _agentRepository.UpdateAgent(agent);

                return userWasUpdated;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Atualizar usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Atualizar usuário - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Atualizar usuário - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Atualizar usuário - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Atualizar usuário - Dados vazios.");
            }
        }

        private async Task<bool> DeleteAgentRepository(int agentId)
        {
            bool agentWasDeleted = false;

            try
            {
                agentWasDeleted = await _agentRepository.DeleteAgent(agentId);

                return agentWasDeleted;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Excluir Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Excluir Agente - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Excluir Agente - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Excluir Agente - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Excluir Agente - Dados vazios.");
            }
        }

        private async Task<List<AgentDTO>> GetAgentsWithouthFiltersRepository()
        {
            List<AgentDTO> agents = new List<AgentDTO>();

            try
            {
                agents = await _agentRepository.GetAgentsWithouthFilters();

                return agents;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar usuário - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar usuário - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar usuário - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar usuário - Dados vazios.");
            }
        }

        private async Task<List<AgentDTO>> GetAgentsWithFiltersRepository(bool? status, string? name)
        {
            List<AgentDTO> agents = new List<AgentDTO>();

            try
            {
                agents = await _agentRepository.GetAgentsWithFilters(status, name);

                return agents;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Agente - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Agente - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Agente - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Agente - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Agente - Dados vazios.");
            }
        }

        #endregion

    }
}
