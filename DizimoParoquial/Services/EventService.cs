using DizimoParoquial.Data.Interface;

namespace DizimoParoquial.Services
{
    public class EventService
    {

        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        //public async Task<bool> SaveEvent()
        //{

        //}

        #region Repositories Methods

        //private async Task<bool> SaveEventRepository()
        //{

        //}

        #endregion

    }
}
