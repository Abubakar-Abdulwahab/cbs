using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class CheckIsAdministratorFilter : ActionFilterAttribute
    {
        private bool _doIsAdministratorCheck;

        public CheckIsAdministratorFilter(bool doIsAdministratorCheck)
        {
            this._doIsAdministratorCheck = doIsAdministratorCheck;
        }


        public bool DoIsAdministratorCheck()
        {
            return _doIsAdministratorCheck;
        }
    }
}