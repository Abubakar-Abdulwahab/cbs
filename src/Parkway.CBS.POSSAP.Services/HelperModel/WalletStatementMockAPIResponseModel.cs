using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class WalletStatementMockAPIResponseModel
    {
        public bool Error { get; set; }

        public string ErrorMessage { get; set; }

        public List<dynamic> items { get; set; }
    }
}
