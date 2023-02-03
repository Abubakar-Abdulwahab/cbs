using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.PayeeAdapters.IPPIS
{
    public class IPPISAssessmentLineRecordModel : LineRecordModel
    {
        public PayeeStringValue Ministry_Name { get; internal set; }

        public PayeeStringValue Period_Name { get; internal set; }

        public PayeeStringValue Org_Code { get; internal set; }

        public PayeeStringValue Employee_Number { get; internal set; }

        public PayeeStringValue Employee_Name { get; internal set; }

        public PayeeStringValue Grade_Level { get; internal set; }

        public PayeeStringValue Step { get; internal set; }

        public PayeeStringValue Tax_State { get; internal set; }

        public PayeeStringValue Contact_Address { get; internal set; }

        public PayeeStringValue Email_Address { get; internal set; }

        public PayeeStringValue Mobile_Phone { get; internal set; }

        public PayeeDecimalValue Tax { get; set; }
    }

}
