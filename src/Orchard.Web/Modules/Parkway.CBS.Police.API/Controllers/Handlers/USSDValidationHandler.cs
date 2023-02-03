using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Net;
using System.Text;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class USSDValidationHandler : IUSSDRequestTypeHandler
    {
        public ILogger Logger { get; set; }
        public USSDRequestType GetRequestType => USSDRequestType.Validation;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<IPoliceCollectionLogManager<PoliceCollectionLog>> _policeCollectionLog;
        private readonly Lazy<IInvoiceManager<Invoice>> _invoiceManager;
        private const int DocumentNumberOption = 1;
        private const int ReceiptNumberOption = 2;
        private const int InvoiceNumberOption = 3;
        private const int ValidationItemsListStage = 1;
        private const int ItemSelectionStage = 2;

        public USSDValidationHandler(Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<IPoliceCollectionLogManager<PoliceCollectionLog>> policeCollectionLog, Lazy<IInvoiceManager<Invoice>> invoiceManager)
        {
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _policeCollectionLog = policeCollectionLog;
            _invoiceManager = invoiceManager;
        }

        /// <summary>
        /// Process ussd validation request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse StartRequest(USSDRequestModel model)
        {
            try
            {
                string[] requestStage = model.Text.Split('|');
                switch (requestStage.Length)
                {
                    case ValidationItemsListStage:
                        return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = ValidationItemsMenu() };

                    case ItemSelectionStage:
                        return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = ItemSelectionPrompt(model) };
                }

                //If the stage length is greater than item selection stage, then it is user entered value stage
                if (requestStage.Length > ItemSelectionStage)
                {
                    return ValidateItem(model);
                }

                throw new DirtyFormDataException("Invalid input, please try again.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Present a menu for a user to select an item to be validated
        /// </summary>
        /// <returns></returns>
        private string ValidationItemsMenu()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Welcome to POSSAP Service.\n");
            sb.Append("Please select the item you will like to validate\n");
            sb.Append("\n");
            sb.Append("1.Document Number\n");
            sb.Append("2.Receipt Number\n");
            sb.Append("3.Invoice Number\n");

            return sb.ToString();
        }

        /// <summary>
        /// Present a menu to enter the value for a specific item to be validated
        /// </summary>
        /// <returns></returns>
        private string ItemSelectionPrompt(USSDRequestModel model)
        {
            string[] requestStage = model.Text.Split('|');
            if (string.IsNullOrEmpty(requestStage[1]))
            {
                throw new DirtyFormDataException("Invalid input, please try again.");
            }
            bool parsed = int.TryParse(requestStage[1], out int itemTypeId);
            if (!parsed)
            {
                throw new DirtyFormDataException("Invalid input, please try again.");
            }

            switch (itemTypeId)
            {
                case DocumentNumberOption:
                    return "Please enter the Document Number";

                case ReceiptNumberOption:
                    return "Please enter the Receipt Number";

                case InvoiceNumberOption:
                    return "Please enter the Invoice Number";
            }
            throw new DirtyFormDataException("Invalid input, please try again.");
        }

        /// <summary>
        /// Validate the user selected item
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private USSDAPIResponse ValidateItem(USSDRequestModel model)
        {
            string[] requestStage = model.Text.Split('|');
            if (string.IsNullOrEmpty(requestStage[1]))
            {
                throw new DirtyFormDataException("Invalid input, please try again.");
            }
            bool parsed = int.TryParse(requestStage[1], out int itemTypeId);
            if (!parsed)
            {
                throw new DirtyFormDataException("Invalid input, please try again.");
            }

            switch (itemTypeId)
            {
                case DocumentNumberOption:
                    return ValidateDocumentNumber(requestStage[2]);

                case ReceiptNumberOption:
                    return ValidateReceiptNumber(requestStage[2]);

                case InvoiceNumberOption:
                    return ValidateInvoiceNumber(requestStage[2]);
            }
            throw new DirtyFormDataException("Invalid input, please try again.");
        }

        /// <summary>
        /// Validate Document Number
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <returns>string</returns>
        private USSDAPIResponse ValidateDocumentNumber(string approvalNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(approvalNumber))
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                USSDValidateDocumentVM validatedInfo = _requestManager.Value.GetRequestDetailsByApprovalNumber(approvalNumber);
                if (validatedInfo == null)
                {
                    throw new NoRecordFoundException("The specified document number not found.");
                }

                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = validatedInfo };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Validate Receipt Number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>string</returns>
        private USSDAPIResponse ValidateReceiptNumber(string receiptNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(receiptNumber))
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                USSDValidateReceiptVM validatedInfo = _policeCollectionLog.Value.GetReceiptInfo(receiptNumber);
                if (validatedInfo == null)
                {
                    throw new NoRecordFoundException("The specified receipt number not found.");
                }

                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = validatedInfo };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Validate Receipt Number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>string</returns>
        private USSDAPIResponse ValidateInvoiceNumber(string invoiceNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                ValidateInvoiceVM validatedInfo = _invoiceManager.Value.GetInvoiceInfo(invoiceNumber);
                if (validatedInfo == null)
                {
                    throw new NoRecordFoundException("The specified invoice number not found.");
                }

                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = new { validatedInfo.InvoiceNumber, validatedInfo.ApplicantName, validatedInfo.InvoiceAmount, validatedInfo.AmountPaid, Status = validatedInfo.Status.ToDescription() } };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}