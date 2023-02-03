using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Client.Web.ViewModels
{
    public class ReportModelForPayeAssessment
    {
        public List<PayeeReturnModelVM> ReportItems { get; set; }

        public DirectAssessmentReportVM Report { get; set; }

    }
}