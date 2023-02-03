using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{

    public class PaginationModel
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int ChunkSize { get; set; }
    }


    /// <summary>
    /// Model helper for API requests
    /// </summary>
    public class ReceiptDataModel : PaginationModel
    {
        public long TaxEntityId { get; set; }

        public string DateFilter { get; set; }

        public string ReceiptNumber { get; set; }

        public bool IsEmployer { get; set; }
    }


    /// <summary>
    /// Model helper for API requests
    /// </summary>
    public class PAYEBatchRecordDataModel : PaginationModel
    {
        public long TaxEntityId { get; set; }

        public string DateFilter { get; set; }

        public string BatchRef { get; set; }
    }


    public class FileProcessModel
    {
        public Int64 Id { get; set; }

        [Obsolete]
        public DirectAssessmentBatchRecord DirectAssessmentBatchRecord { get; set; }

        public dynamic FileBatchRecord { get; set; }

        public bool NoFilePresent { get; set; }

        public bool NoSchedulePresent { get; set; }

        public long BatchRecordId { get; set; }
    }


    public class DirectAssessmentReportVM
    {
        public int PageSize { get; set; }

        public string Amount { get; set; }

        public List<PayeeReturnModelVM> Payees { get; set; }

        public FileUploadReport PayeeExcelReport { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string Category { get; set; }

        public string RevenueHeadName { get; set; }

        public string MDAName { get; set; }

        public string Recipient { get; set; }

        public string TIN { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string AdapterValue { get; set; }

        public bool DoWork { get; set; }

        public string ErrorMessage { get; set; }

        public string PayerId { get; set; }

        public string ExternalRef { get; set; }

        public string ActionViewPath { get; set; }
    }


    public class PayeeExcelReport : FileUploadReport
    {

    }


    public class PayeeReturnModelVM
    {
        public string PayeeName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string LGA { get; set; }

        public string Address { get; set; }

        public string GrossAnnual { get; set; }

        public string Exemptions { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        public string TaxableIncome { get; set; }

        public string TIN { get; set; }

        public string PayerId { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}