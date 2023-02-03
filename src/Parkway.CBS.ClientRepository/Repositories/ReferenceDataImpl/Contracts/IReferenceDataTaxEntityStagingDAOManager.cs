using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IReferenceDataTaxEntityStagingDAOManager
    {
        /// <summary>
        /// Update the Tax Entity records where the Tax Entity Category and PhoneNumber matches what is in the Reference Data records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateTaxEntityWithReferenceDataRecords(long batchId);


        /// <summary>
        /// Create the Tax Entity records where the Tax Entity Category and PhoneNumber does not match what is in the Reference Data records.
        /// </summary>
        /// <param name="batchId"></param>
        void CreateTaxEntityWithReferenceDataRecords(long batchId);

        /// <summary>
        /// Update the Tax Entity Staging records operationTypeId where the Tax Entity Category and PhoneNumber matches what is in the Reference Data records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateReferenceDataTaxEntityStagingRecordsOperationType(long batchId);

        /// <summary>
        /// Update the Tax Entity Staging records operationTypeId where the Tax Entity Category and PhoneNumber matches what is in the Reference Data records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateReferenceDataTaxEntityStagingRecordsTaxEntityId(long batchId);

        /// <summary>
        /// Move the tax entity staging records to the WithHolding Tax on Rent table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent(long batchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        void MoveRecordsToAssets(long batchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        void MoveWithHoldingTaxOnRentRecordsToInvoiceStaging(long batchId);


        /// <summary>
        /// Update the Tax Entity customer details records with the customer details response from the Cashflow .
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateTaxEntityWithCashflowInvoiceResponse(long batchId);

        /// <summary>
        /// 
        /// </summary>
        void MoveDevelopmentLevyRecordToInvoiceStagingTable(long batchId);

    }
}
