using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using OrchardPermission = Orchard.Security.Permissions.Permission;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers
{
    public class PAYEReceiptUtilizationHandler : IPAYEReceiptUtilizationHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IPAYEReceiptUtilizationFilter _payeReceiptUtilizationFilter;
        public ILogger Logger { get; set; }

        public PAYEReceiptUtilizationHandler(IOrchardServices orchardServices, IPAYEReceiptUtilizationFilter payeReceiptUtilizationFilter)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            Logger = NullLogger.Instance;
            _payeReceiptUtilizationFilter = payeReceiptUtilizationFilter;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="CanViewReceiptUtilizations"></param>
        public void CheckForPermission(OrchardPermission CanViewReceiptUtilizations)
        {
            IsAuthorized(CanViewReceiptUtilizations);
        }

        /// <summary>
        /// Check if the user is authorized to perform an action
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void IsAuthorized(OrchardPermission permission)
        {
            if (!_authorizer.Authorize(permission, ErrorLang.usernotauthorized()))
                throw new UserNotAuthorizedForThisActionException();
        }


        /// <summary>
        /// Get PAYE receipts list
        /// </summary>
        public PAYEReceiptReportVM GetPAYEReceipts(PAYEReceiptSearchParams receiptSearchParams)
        {
            var receiptObj = _payeReceiptUtilizationFilter.GetReceiptViewModel(receiptSearchParams);

            return new PAYEReceiptReportVM
            {
                From = receiptSearchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = receiptSearchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                PayerId = receiptSearchParams.PayerId,
                Status = receiptSearchParams.Status,
                TotalRequestRecord = (int)(((IEnumerable<ReportStatsVM>)receiptObj.Aggregate).First().TotalRecordCount),
                ReceiptNumber = receiptSearchParams.ReceiptNumber,
                ReceiptItems = (IEnumerable<PAYEReceiptVM>)receiptObj.ReceiptRecords,
            };
        }


        /// <summary>
        /// Gets receipt utilizations report for a specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public PAYEReceiptUtilizationReportObj GetUtilizationsReport(string receiptNumber)
        {
            try
            {
                var receiptUtilizationsObj = _payeReceiptUtilizationFilter.GetReceiptUtilizations(receiptNumber);
                return new PAYEReceiptUtilizationReportObj
                {
                    ReceiptNumber = receiptNumber,
                    ReceiptUtilizationItems = receiptUtilizationsObj,
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}