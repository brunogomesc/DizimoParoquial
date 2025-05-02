using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IEventRepository
    {

        public Task<bool> SaveEvent(Event newEvent);

    }
}
