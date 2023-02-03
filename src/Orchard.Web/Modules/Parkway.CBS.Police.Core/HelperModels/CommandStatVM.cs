using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CommandStatVM
    {
        public int NumberofSuccessfulRecords { get; set; }

        public int NumberofUnSuccessfulRecords { get; set; }

        public List<CommandVM> UnSuccessfulRecords { get; set; }

    }
}