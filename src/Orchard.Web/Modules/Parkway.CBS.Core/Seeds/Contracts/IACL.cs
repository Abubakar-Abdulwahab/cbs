using Orchard;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface IACL : IDependency
    {
        void CreateRoles();

        void CreatePermissions();

        void CreateUserRole();

        void CreateRolePermission();
    }
}