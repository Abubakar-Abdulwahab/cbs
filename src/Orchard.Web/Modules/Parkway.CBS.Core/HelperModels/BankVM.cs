using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class BankVM
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }
        public string PayDirectBankCode { get; set; }
        public string Id { get; set; }
    }
}