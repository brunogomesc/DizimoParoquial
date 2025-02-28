using DizimoParoquial.DTOs;
using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IPermissionRepository
    {

        public Task<List<UserPermissionDTO>> GetUserPermissions(int user);

        public Task<List<Permission>> GetAllPermissions();

        public Task<bool> RegisterPermissions(int userId, List<int> selectedPermissionsInsertion);

        public Task<bool> DeleteAllPermissionsByUser(int userId);

    }
}
