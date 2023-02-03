using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class GenericPoliceServices : IPSSServiceTypeImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.GenericPoliceServices;

        private readonly ITypeImplComposer _compositionHandler;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreCommand _coreCommand;


        public GenericPoliceServices(ICoreCommand coreCommand, ITypeImplComposer compositionHandler, IOrchardServices orchardServices)
        {
            _coreCommand = coreCommand;
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// This method gets the model for displaying the confirmation details of a request 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="objStringValue"></param>
        /// <returns>RequestConfirmationVM</returns>
        public RequestConfirmationVM GetModelForRequestConfirmation(int serviceId, string objStringValue)
        {
            IEnumerable<UserFormDetails> formValues = JsonConvert.DeserializeObject<IEnumerable<UserFormDetails>>(objStringValue);
            if (formValues == null)
            {
                formValues = new List<UserFormDetails> { };
            }
            //get init level definition
            //here we get the init definition level, this line would require some modifications
            //when the workflow direction of this service is different from what was assigned
            //we will need to pass in some parameters such as the HasDifferentialWorkFlow
            //for generic we would need to modify the objstring value to hold proper data such as the properties
            //in the request dump, this is done here, would need work if we have workflow that might vary
            //for different variants or flows for the service
            int initLevelId = _compositionHandler.GetInitFlow(serviceId, false, new ServiceWorkFlowDifferentialDataParam { });
            IEnumerable<PSServiceRevenueHeadVM> result = _compositionHandler.GetRevenueHeadDetails(serviceId, initLevelId);

            if (result == null || !result.Any())
            {
                throw new NoBillingInformationFoundException("No billing info found for service Id " + serviceId);
            }


            return new RequestConfirmationVM
            {
                AmountDetails = result.Where(r => !r.IsGroupHead).Select(r => new AmountDetails { AmountToPay = (r.AmountToPay + r.Surcharge), FeeDescription = r.FeeDescription }).ToList(),
                HeaderObj = new HeaderObj { },
                ServiceRequested = result.ElementAt(0).ServiceName,
                FormValues = formValues.ToList(),
            };
        }



        /// <summary>
        /// Save request details for this request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        public InvoiceGenerationResponse SaveRequestDetailsAfterConfirmation(PSSRequestStageModel processStage, string sRequestFormDump, TaxEntityViewModel taxPayerProfileVM)
        {
            try
            {
                //we want to save this request
                IEnumerable<UserFormDetails> formValues = JsonConvert.DeserializeObject<IEnumerable<UserFormDetails>>(sRequestFormDump);
                if (formValues == null)
                {
                    formValues = new List<UserFormDetails> { };
                }

                //do work for request
                return _compositionHandler.SaveRequestDetails(processStage, new RequestDumpVM { SelectedCommand = GetBaseCommand(), SiteName = _orchardServices.WorkContext.CurrentSite.SiteName, DontValidateFormControls = true,}, taxPayerProfileVM, PSSRequestStatus.PendingInvoicePayment, formValues);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("{0} {1}", exception.Message, sRequestFormDump));
                _compositionHandler.RollBackAllTransactions();
                throw;
            }
        }
        


        /// <summary>
        /// return Id of the force headquaters command
        /// </summary>
        /// <returns>int</returns>
        /// <exception cref="NoRecordFoundException">If value is less than 1 method throws this error</exception>
        private int GetBaseCommand()
        {
            int value = _coreCommand.GetFederalCommand().Id;
            if (value < 1) throw new NoRecordFoundException("Could not find base command");
            return value;
        }
    
    }
}