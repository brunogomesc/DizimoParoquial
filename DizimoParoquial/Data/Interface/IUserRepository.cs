using DizimoParoquial.DTOs;
using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IUserRepository
    {

        public Task<UserDTO> GetUserByUsernameAndPassword(string username, string password);

        public Task<bool> RegisterUser(User user);

        public Task<List<UserDTO>> GetUsersWithouthFilters();

        public Task<List<UserDTO>> GetUsersWithFilters(bool? status, string? name);

        public Task<UserDTO> GetUserByUsername(string username);

        public Task<bool> DeleteUser(int userId);

        public Task<UserDTO> GetUserById(int id);

        public Task<bool> UpdateUser(User user);

    }
}
