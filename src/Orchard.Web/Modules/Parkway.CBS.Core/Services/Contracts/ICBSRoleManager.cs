using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ICBSRoleManager<CBSRole> : IDependency, IBaseManager<CBSRole>
    {
    }

    public interface ICBSPermissionManager<CBSPermission> : IDependency, IBaseManager<CBSPermission>
    {
    }
    public interface ICBSUserRoleManager<CBSUserRole> : IDependency, IBaseManager<CBSUserRole>
    {
    }
    public interface ICBSRolePermissionManager<CBSRolePermission> : IDependency, IBaseManager<CBSRolePermission>
    {
    }
}
