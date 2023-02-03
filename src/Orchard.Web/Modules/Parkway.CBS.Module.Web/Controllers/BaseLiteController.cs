using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Web.Mvc;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Module.Web.Controllers
{
    public abstract class BaseLiteController : Controller
    {
        private readonly IHandlerComposition _handlerComposition;
        public ILogger Logger { get; set; }


        public BaseLiteController(IHandlerComposition handlerComposition)
        {
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return null if cbs user is not found</para>
        /// </summary>
        /// <returns>UserDetailsModel | null</returns>
        protected virtual UserDetailsModel GetLoggedInUserDetails(bool checkVerifiedGate = false)
        {
            try
            {
                return _handlerComposition.GetLoggedInUserDetails();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return null;
        }


        /// <summary>
        /// Get deserialized obj that has the stage model
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>GenerateInvoiceStepsModel</returns>
        /// <exception cref="Exception"></exception>
        protected virtual GenerateInvoiceStepsModel GetDeserializedSessionObj(ref string errorMessage)
        {
            try { return JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString()); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception getting the deserializing the session value {0}", exception.Message));
                errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                throw new Exception(errorMessage);
            }
        }


    }

}