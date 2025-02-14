using DizimoParoquial.Data.Interface;
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

        public async Task<User> GetUserByUsernameAndPassword(string username, string password)
        {

            User user = new User();

            try
            {
                string passwordEncrypted = _encryption.EncryptPassword(password, _configurationService.GetKeyEncryption());

                user = await GetUserByUsernameAndPasswordRepository(username, passwordEncrypted);

                return user;

            }
            catch (Exception)
            {

                throw;
            }

        }

        #region Repositories Methods

        private async Task<User> GetUserByUsernameAndPasswordRepository(string username, string password)
        {
            User user = new User();

            try
            {
                user = await _userRepository.GetUserByUsernameAndPassword(username, password);

                return user;
            }
            catch (Exception ex)
            {
                return user;
            }
        }

        #endregion

    }
}
