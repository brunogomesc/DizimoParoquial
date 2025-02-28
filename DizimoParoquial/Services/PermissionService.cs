using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Utils;
using System.Data.Common;

namespace DizimoParoquial.Services
{
    public class PermissionService
    {

        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<UserPermissionDTO>> GetUserPermissions(int user)
        {
            List<UserPermissionDTO> userPermissions = new List<UserPermissionDTO>();

            try
            {
                userPermissions = await GetUserPermissionsRepository(user);
                return userPermissions;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Permissões - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Permissões - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Permissões - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Permissões - Dados vazios.");
            }
        }

        public async Task<List<Permission>> GetAllPermissions()
        {
            List<Permission> permissions = new List<Permission>();

            try
            {
                permissions = await GetAllPermissionsRepository();
                return permissions;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Todas Permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Todas Permissões - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Todas Permissões - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Todas Permissões - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Todas Permissões - Dados vazios.");
            }
        }

        public async Task<bool> UpdatePermissions(int userId, List<int> selectedPermissionsScreen)
        {
            bool permissionsWereUpdated = false;

            try
            {

                List<UserPermissionDTO> currentUserPermissions = await GetUserPermissions(userId);

                if(currentUserPermissions?.Count() > 0)
                    permissionsWereUpdated = await DeleteAllPermissionsByUserRepository(userId);

                if (permissionsWereUpdated || !(currentUserPermissions?.Count() > 0))
                    permissionsWereUpdated = await RegisterPermissionsRepository(userId, selectedPermissionsScreen);

                return permissionsWereUpdated;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Atualizar permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Atualizar permissões - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Atualizar permissões - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Atualizar permissões - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Atualizar permissões - Dados vazios.");
            }
        }

        #region Repositories Methods

        private async Task<List<UserPermissionDTO>> GetUserPermissionsRepository(int user)
        {
            List<UserPermissionDTO> userPermissions = new List<UserPermissionDTO>();

            try
            {
                userPermissions = await _permissionRepository.GetUserPermissions(user);

                return userPermissions;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Permissões - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Permissões - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Permissões - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Permissões - Dados vazios.");
            }
        }

        private async Task<List<Permission>> GetAllPermissionsRepository()
        {
            List<Permission> permissions = new List<Permission>();

            try
            {
                permissions = await _permissionRepository.GetAllPermissions();

                return permissions;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Todas Permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Todas Permissões - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Todas Permissões - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Todas Permissões - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Todas Permissões - Dados vazios.");
            }
        }

        private async Task<bool> RegisterPermissionsRepository(int userId, List<int> selectedPermissionsInsertion)
        {
            bool permissionsWereRegistered = false;

            try
            {
                permissionsWereRegistered = await _permissionRepository.RegisterPermissions(userId, selectedPermissionsInsertion);

                return permissionsWereRegistered;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar permissões - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar permissões - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar permissões - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar permissões - Dados vazios.");
            }
        }

        private async Task<bool> DeleteAllPermissionsByUserRepository(int userId)
        {
            bool permissionsWereDeleted = false;

            try
            {
                permissionsWereDeleted = await _permissionRepository.DeleteAllPermissionsByUser(userId);

                return permissionsWereDeleted;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Excluir permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Excluir permissões - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Excluir permissões - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Excluir permissões - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Excluir permissões - Dados vazios.");
            }
        }

        #endregion

    }
}
