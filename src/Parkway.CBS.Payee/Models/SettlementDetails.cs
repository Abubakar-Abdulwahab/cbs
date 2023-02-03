using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.Models
{
    public class SettlementDetails
    {
        public int Spacing { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public List<SettlementParty> Parties { get; set; }
    }
}
