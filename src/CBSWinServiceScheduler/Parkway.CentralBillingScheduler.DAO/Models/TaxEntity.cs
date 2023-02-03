using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public class TaxEntity : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }
        public virtual string Recipient { get; set; }
        public virtual string Email { get; set; }
        public virtual string Address { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual int TaxEntityType { get; set; }
        public virtual TaxEntityCategory TaxEntityCategory { get; set; }
        public virtual string TaxPayerIdentificationNumber { get; set; }
        public virtual string Occupation { get; set; }
        public virtual long PrimaryContactId { get; set; }
        public virtual long CashflowCustomerId { get; set; }

        public override string ToString()
        {
            return string.Format("Email - {0}, TIN - {1}, PhoneNumber - {2} ", Email,
                TaxPayerIdentificationNumber, PhoneNumber);
        }
    }
}