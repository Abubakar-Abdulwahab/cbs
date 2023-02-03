using System.Web.Mvc;


namespace Parkway.CBS.Police.Client.Middleware
{
    public class CheckVerificationFilter : ActionFilterAttribute
    {
        private bool _doVerificationCheck;

        public CheckVerificationFilter(bool doVerificationCheck)
        {
            this._doVerificationCheck = doVerificationCheck;
        }


        public bool DoVerificationCheck()
        {
            return _doVerificationCheck;
        }
    }
}