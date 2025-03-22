using DizimoParoquial.Data.Interface;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Utils;
using System.Data.Common;

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
            catch (DbException ex)
            {
                throw new RepositoryException("Login - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Login - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Login - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Login - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Login - Dados vazios.");
            }

        }

        public async Task<bool> RegisterUser(User user)
        {
            bool userWasCreated = false;

            try
            {

                UserDTO userExists = await GetUserByUsernameRepository(user.Username);

                if (userExists != null && userExists.UserId > 0)
                    throw new ValidationException("Usuário já existe, por favor cadastrar novo usuário!");

                user.Active = true;
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = null;
                user.Password = _encryption.EncryptPassword(user.Password, _configurationService.GetKeyEncryption());

                userWasCreated = await RegisterUserRepository(user);

                return userWasCreated;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar usuário - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar usuário - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar usuário - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar usuário - Dados vazios.");
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            bool userWasUpdated = false;

            try
            {

                UserDTO userExists = await GetUserByIdRepository(user.UserId);

                if (userExists == null && userExists.UserId == 0)
                    throw new ValidationException("Usuário não existente");

                if (user.Password == null)
                    user.Password = userExists.Password;
                else
                    user.Password = _encryption.EncryptPassword(user.Password, _configurationService.GetKeyEncryption());

                user.UpdatedAt = DateTime.Now;

                userWasUpdated = await UpdateUserRepository(user);

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

        public async Task<List<UserDTO>> GetUsersWithFilters(bool? status, string? name)
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {

                users = await GetUsersWithFiltersRepository(status, name);

                return users;

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

        public async Task<List<UserDTO>> GetUsersWithouthFilters()
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {
                
                users = await GetUsersWithouthFiltersRepository();

                return users;

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

        public async Task<bool> DeleteUser(int userId)
        {
            bool userWasDeleted = false;

            try
            {
                userWasDeleted = await DeleteUserRepository(userId);
                

                return userWasDeleted;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Excluir usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Excluir usuário - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Excluir usuário - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Excluir usuário - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Excluir usuário - Dados vazios.");
            }
        }

        public async Task<UserDTO> GetUserById(int id)
        {
            UserDTO user = new UserDTO();
            try
            {
                user = await GetUserByIdRepository(id);
                return user;
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

        #region Repositories Methods

        private async Task<UserDTO> GetUserByUsernameAndPasswordRepository(string username, string password)
        {
            UserDTO user = new UserDTO();

            try
            {
                user = await _userRepository.GetUserByUsernameAndPassword(username, password);

                return user;
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

        private async Task<bool> RegisterUserRepository(User user)
        {
            bool userWasCreated = false;

            try
            {
                userWasCreated = await _userRepository.RegisterUser(user);

                return userWasCreated;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar usuário - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar usuário - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar usuário - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar usuário - Dados vazios.");
            }
        }

        private async Task<bool> UpdateUserRepository(User user)
        {
            bool userWasUpdated = false;

            try
            {
                userWasUpdated = await _userRepository.UpdateUser(user);

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

        private async Task<List<UserDTO>> GetUsersWithouthFiltersRepository()
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {
                users = await _userRepository.GetUsersWithouthFilters();

                return users;
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

        private async Task<List<UserDTO>> GetUsersWithFiltersRepository(bool? status, string? name)
        {
            List<UserDTO> users = new List<UserDTO>();

            try
            {
                users = await _userRepository.GetUsersWithFilters(status, name);

                return users;
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

        private async Task<UserDTO> GetUserByUsernameRepository(string username)
        {
            UserDTO user = new UserDTO();

            try
            {
                user = await _userRepository.GetUserByUsername(username);

                return user;
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

        private async Task<bool> DeleteUserRepository(int userId)
        {
            bool userWasDeleted = false;

            try
            {
                userWasDeleted = await _userRepository.DeleteUser(userId);

                return userWasDeleted;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Excluir usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Excluir usuário - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Excluir usuário - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Excluir usuário - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Excluir usuário - Dados vazios.");
            }
        }

        private async Task<UserDTO> GetUserByIdRepository(int id)
        {
            UserDTO user = new UserDTO();

            try
            {
                user = await _userRepository.GetUserById(id);

                return user;
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
                throw new NullException("AtuaConsultarlizar usuário - Dados vazios.");
            }
        }

        #endregion

    }
}
