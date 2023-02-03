using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Parkway.CBS.FileUpload
{
    public class ValidationObject
    {
        public bool HasError { get; set; }

        public string ErrorMessages { get; set; }

        public string Value { get; set; }

        public int IntValue { get; set; }
    }


    public class OSGOFCellSiteModelV2
    {
        public string ErrorMessages { get; set; }

        public bool HasError { get; set; }

        public string CellSiteValue { get; set; }

        public DateTime? AssessmentDate { get; set; }

        public string Ref { get; set; }

        public int Year { get; set; }

        public string YearStringValue { get; set; }
    }


    public class OSGOFCellSiteModel
    {
        public string ErrorMessages { get; set; }

        public bool HasError { get; set; }

        public string StateValue { get; set; }

        public string StateName { get; set; }

        public string LGAValue { get; set; }

        public string LGAId { get; set; }

        public string CellSiteValue { get; set; }

        public string CellSiteCode { get; set; }

        public string AmountValue { get; set; }

        public string Address { get; set; }

        public string Coords { get; set; }

        public decimal Amount { get; set; }

        public DateTime? AssessmentDate { get; set; }

        public string Ref { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }
    }


    public class CellSitesBreakDown
    {
        public decimal TotalAmount { get; set; }

        public ConcurrentStack<OSGOFCellSiteModel> CellSiteModel { get; set; }

        public ConcurrentStack<OSGOFCellSiteModelV2> CellSiteModelV2 { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public string TotalAmountValue { get; set; }
    }
}