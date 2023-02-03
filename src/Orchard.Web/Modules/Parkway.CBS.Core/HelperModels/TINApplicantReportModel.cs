using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TINApplicantReportModel
    {
        public long TINId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string TINRef { get; set; }
        public string TIN { get; set; }
        public DateTime DateOfRegistration { get; set; }

        public dynamic Pager { get; set; }
        public IList<dynamic> TableData { get; set; }

        public TINApplicantReportModel()
        {

        }

        public TINApplicantReportModel(IEnumerable<dynamic> tableData, dynamic pager)
        {
            Pager = pager;
            TableData = tableData.ToArray();
        }

    }
}