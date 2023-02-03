using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class TaxPayerDetailsViewModel
    {
        public dynamic Pager { get; set; }

        public Int64 Id { get; set; }

        public string TIN { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Email { get; set; }
        public string PayerId { get; set; }
        public string TaxPayerCode { get; set; }
        public DateTime DateCreated { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<AccountStatementLogModel> Report { get; set; }

        public decimal TotalCreditAmount { get; set; }
        public decimal TotalBillAmount { get; set; }


        [StringLength(10, ErrorMessage = "Enter a valid date range. Date format dd/MM/yyyy", MinimumLength = 10)]
        /// <summary>
        /// Format dd/MM/yyyy
        /// </summary>
        public string FromRange { get; set; }

        [StringLength(10, ErrorMessage = "Enter a valid date range. Date format dd/MM/yyyy", MinimumLength = 10)]
        /// <summary>
        /// Format dd/MM/yyyy
        /// </summary>
        public string EndRange { get; set; }

        public int PaymentTypeId { get; set; }

    }
}