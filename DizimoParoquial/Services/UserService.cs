using DizimoParoquial.Data.Interface;
using DizimoParoquial.DTOs;
using DizimoParoquial.Models;
using DizimoParoquial.Utils;

namespace DizimoParoquial.Services
{
    public class UserService
    {

        private readonly IUserRepository _userRepository;
        private readonly Encryption _encryption;
        private readonly ConfigurationService _configurationService;

        public UserService(IUserRepository userRepository, Encryption encryption, ConfigurationService configurationService)
        {
            _userRepository = userRepository;
            _encryption = encryption;
            _configurationService = configurationService;
        }

        public async Task<UserDTO> GetUserByUsernameAndPassword(string username, string password)
        {

            UserDTO user = new UserDTO();

            try
            {
                string passwordEncrypted = _encryption.EncryptPassword(password, _configurationService.GetKeyEncryption());

                user = await GetUserByUsernameAndPasswordRepository(username, passwordEncrypted);

                return user;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<bool> RegisterUser(User user)
        {
            bool userWasCreated = false;

            try
            {

                UserDTO userExists = await GetUserByUsernameRepository(user.Username);

                if (userExists != null && userExists.UserId > 0)
                    return userWasCreated;

                user.Active = true;
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = null;
                user.Password = _encryption.EncryptPassword(user.Password, _configurationService.GetKeyEncryption());

                userWasCreated = await RegisterUserRepository(user);

                return userWasCreated;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserDTO>> GetUsersWithFilters(bool? status, string? name)
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {

                users = await GetUsersWithFiltersRepository(status, name);

                return users;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserDTO>> GetUsersWithouthFilters()
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {
                
                users = await GetUsersWithouthFiltersRepository();

                return users;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            bool userWasDeleted = false;

            try
            {
                userWasDeleted = await DeleteUserRepository(userId);
                

                return userWasDeleted;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Repositories Methods

        private async Task<UserDTO> GetUserByUsernameAndPasswordRepository(string username, string password)
        {
            UserDTO user = new UserDTO();

            try
            {
                user = await _userRepository.GetUserByUsernameAndPassword(username, password);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<bool> RegisterUserRepository(User user)
        {
            bool userWasCreated = false;

            try
            {
                userWasCreated = await _userRepository.RegisterUser(user);

                return userWasCreated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<UserDTO>> GetUsersWithouthFiltersRepository()
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {
                users = await _userRepository.GetUsersWithouthFilters();

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<UserDTO>> GetUsersWithFiltersRepository(bool? status, string? name)
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {
                users = await _userRepository.GetUsersWithFilters(status, name);

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<UserDTO> GetUserByUsernameRepository(string username)
        {
            UserDTO user = new UserDTO();

            try
            {
                user = await _userRepository.GetUserByUsername(username);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<bool> DeleteUserRepository(int userId)
        {
            bool userWasDeleted = false;

            try
            {
                userWasDeleted = await _userRepository.DeleteUser(userId);

                return userWasDeleted;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

    }
}
