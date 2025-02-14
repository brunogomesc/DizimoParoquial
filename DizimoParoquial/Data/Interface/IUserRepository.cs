using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IUserRepository
    {

        public Task<User> GetUserByUsernameAndPassword(string username, string password);

    }
}
