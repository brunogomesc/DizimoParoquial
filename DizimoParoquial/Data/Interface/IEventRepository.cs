using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IEventRepository
    {

        public Task<bool> SaveEvent(Event newEvent);

        public Task<List<ReportEvent>> GetReportEvents(string? agentName, DateTime startEventDate, DateTime endEventDate);

    }
}
