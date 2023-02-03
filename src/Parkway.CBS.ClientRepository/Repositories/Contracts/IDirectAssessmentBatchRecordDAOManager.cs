using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IDirectAssessmentBatchRecordDAOManager : IRepository<DirectAssessmentBatchRecord>
    {
        /// <summary>
        /// When invoices have been generated from the invoicing service
        /// Lets create the direct assessment for them
        /// </summary>
        /// <param name="batchId"></param>
        void CreateDirectAssessmentsForIPPIS(long batchId, RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails, int month, int year);

        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        void SetInvoiceConfirmationForIPPISToTrue(int month, int year);
    }
}
