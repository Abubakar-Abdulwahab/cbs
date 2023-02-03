using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class BankDetails : CBSModel
    {
        public virtual int BankId { get; set; }

        [Required(ErrorMessage = "Bank account number field is required")]
        public virtual string BankAccountNumber { get; set; }

        public virtual string BankCode { get; set; }
    }
}