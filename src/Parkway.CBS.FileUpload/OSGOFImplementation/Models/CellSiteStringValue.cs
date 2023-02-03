using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.OSGOFImplementation.Models
{
    public class CellSiteStringValue : BaseCellSiteValue
    {
        public string Value { get; internal set; }
        public int ValueId { get; internal set; }
    }

    public class CellSiteIntValue : BaseCellSiteValue
    {
        public int Value { get; internal set; }
        public string StringValue { get; internal set; }
    }


    public class CellSiteDecimalValue : BaseCellSiteValue
    {
        public decimal Value { get; internal set; }

        public string StringValue { get; internal set; }
    }

    public abstract class BaseCellSiteValue
    {
        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; }
    }



}
