using DizimoParoquial.DTOs;
using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IAgentRepository
    {

        public Task<List<string>> GetAgentCodes();

        public Task<bool> RegisterAgent(Agent agent);

        public Task<AgentDTO> GetAgentById(int id);

        public Task<AgentDTO> GetAgentByCode(string agentCode);

        public Task<bool> UpdateAgent(Agent agent);

        public Task<bool> DeleteAgent(int agentId);

        public Task<List<AgentDTO>> GetAgentsWithouthFilters();

        public Task<List<AgentDTO>> GetAgentsWithFilters(bool? status, string? name);

    }
}
