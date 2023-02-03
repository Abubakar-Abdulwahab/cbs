using System.Web.Mvc;
namespace Parkway.CBS.Police.Client.Middleware
{
    public class HasSubUsersFilter : ActionFilterAttribute
    {
        private bool _hasSubUsers;

        public HasSubUsersFilter(bool hasSubUsers)
        {
            this._hasSubUsers = hasSubUsers;
        }


        public bool HasSubUsers()
        {
            return _hasSubUsers;
        }
    }
}