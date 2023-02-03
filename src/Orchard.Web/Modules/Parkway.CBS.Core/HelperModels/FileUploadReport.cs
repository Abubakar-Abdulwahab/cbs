using System;
using Parkway.CBS.Core.Utilities;

namespace Parkway.CBS.Core.HelperModels
{
    public class FileUploadReport
    {
        public Decimal TotalAmountToBePaid { get; set; }

        public int NumberOfValidRecords { get; set; }

        public int NumberOfRecords { get; set; }


        public string StringNumberOfRecords { get { return string.Format("{0:n0}", NumberOfRecords); } }

        public string StringNumberOfRecordsDisplayValue { get { return Util.DisplayFormat(NumberOfRecords); } }

        public string StringNumberOfInvalidRecords { get { return string.Format("{0:n0}", NumberOfRecords - NumberOfValidRecords); } }

        public string StringNumberOfInvalidRecordsDisplayValue { get { return Util.DisplayFormat(NumberOfRecords - NumberOfValidRecords); } }

        public string StringNumberOfValidRecords { get { return string.Format("{0:n0}", NumberOfValidRecords); } }

        public string StringNumberOfValidRecordsDisplayValue { get { return Util.DisplayFormat(NumberOfValidRecords); } }

    }
}