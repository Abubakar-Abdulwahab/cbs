using System.Web.Mvc;

namespace Parkway.CBS.ReadyCash.Go.Middleware.Filters
{
    public class RenderFormFieldsHeaderFilter : ActionFilterAttribute
    {
        private bool _performClientIdAndSignatureValidation;

        public RenderFormFieldsHeaderFilter(bool performClientIdAndSignatureValidation)
        {
            this._performClientIdAndSignatureValidation = performClientIdAndSignatureValidation;
        }


        public bool PerformClientIdAndSignatureValidation()
        {
            return _performClientIdAndSignatureValidation;
        }
    }
}