using System;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEReceiptManager<PAYEReceipt> : IDependency, IBaseManager<PAYEReceipt>
    {
        /// <summary>
        /// Get PAYE Receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PAYEReceiptVM GetPAYEReceiptWithNumber(string receiptNumber, long userId);

        /// <summary>
        /// Get PAYE Receipt with specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PAYEReceiptVM GetPAYEReceipt(Int64 id);

        /// <summary>
        /// Update Utilization Status for PAYE Receipt with the specified Utilization Status
        /// </summary>
        /// <param name="status"></param>
        /// <param name="payeReceiptId"></param>
        /// <returns></returns>
        bool UpdatePAYEReceiptUtilizationStatus(PAYEReceiptUtilizationStatus status, long payeReceiptId);
    }
}