using System;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class DirectAssessmentBatchRecordDAOManager : Repository<DirectAssessmentBatchRecord>, IDirectAssessmentBatchRecordDAOManager
    {
        public DirectAssessmentBatchRecordDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// When invoices have been generated from the invoicing service
        /// Lets create the direct assessment for them
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateDirectAssessmentsForIPPIS(long batchId, RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails, int month, int year)
        {
            try
            {
                //so for the DuplicateComposite we are doing a composite of the  TaxPayerCode,'|', :month,'|', :year, '|', :assessmentType
                //this would help us identify and put a database constraint on the direct assessment table so we do not have duplicate
                //records for the same IPPIS month year and tax payer code processed
                var queryText = $"INSERT INTO Parkway_CBS_Core_DirectAssessmentBatchRecord (TaxEntity_Id, Amount, Billing_Id, RevenueHead_Id, PercentageProgress, TotalNoOfRowsProcessed, Type, CreatedAtUtc, UpdatedAtUtc, Month, Year, PaymentTypeCode, AssessmentType, OriginIdentifier, TaxPayerCode, DuplicateComposite) SELECT TaxEntity_Id, TotalTaxAmount, :billingId, :revenueHeadId, :percentage, NumberofEmployees, :type, :dateSaved, :dateSaved, :month, :year, :paymentTypeCode, :assessmentType, Id, TaxPayerCode, Concat(TaxPayerCode,'|', :month,'|', :year, '|', :assessmentType) FROM Parkway_CBS_Core_IPPISTaxPayerSummary WHERE IPPISBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("billingId", revenueHeadDetails.BillingModelId);
                query.SetParameter("revenueHeadId", revenueHeadDetails.RevenueHeadId);
                query.SetParameter("percentage", 100L);
                query.SetParameter("type", PayeAssessmentType.FileUploadForIPPIS.ToString());
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("month", month);
                query.SetParameter("year", year);
                query.SetParameter("paymentTypeCode", PayeAssessmentType.FileUploadForIPPIS.ToDescription());
                query.SetParameter("assessmentType", PayeAssessmentType.FileUploadForIPPIS);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetInvoiceConfirmationForIPPISToTrue(int month, int year)
        {
            try
            {
                var queryText = $"UPDATE ip SET ip.InvoiceConfirmed = :trueVal FROM Parkway_CBS_Core_DirectAssessmentBatchRecord ip WHERE ip.AssessmentType = :assessmentType AND Month = :month AND Year = :year";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("month", month);
                query.SetParameter("year", year);
                query.SetParameter("assessmentType", PayeAssessmentType.FileUploadForIPPIS);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }
    }
}
