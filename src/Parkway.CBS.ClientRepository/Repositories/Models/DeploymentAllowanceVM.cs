using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.Models
{
    public class DeploymentAllowanceVM
    {
        public Int64 RequestId { get; set; }

        public Int64 InvoiceId { get; set; }

        public int Status { get; set; }

        public int PoliceOfficerId { get; set; }

        public int CommandId { get; set; }

        /// <summary>
        /// Allowance payment amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Amount officer contributed to the total invoice amount
        /// </summary>
        public decimal ContributedAmount { get; set; }

        public string Narration { get; set; }

        public int PaymentStage { get; set; }
    }
}
