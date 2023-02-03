using CBSPay.Core.APIModels;
using CBSPay.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Models
{
    /// <summary>
    /// Save All EIRS Web Payment Request Details
    /// </summary>
    public class EIRSPaymentRequest : BaseEntity<long>
    {
          
         public virtual string ReferenceNumber { get; set; }
         public virtual decimal TotalAmountPaid { get; set; }
         public virtual string Description { get; set; }
         public virtual string TaxPayerName { get; set; }
         public virtual IEnumerable<EIRSPaymentRequestItem> PaymentRequestItems { get; set; }
         public virtual string PaymentIdentifier { get; set; }
         public virtual string PhoneNumber { get; set; }
         public virtual string TaxPayerRIN { get; set; }
         public virtual string TaxPayerTIN { get; set; }
        public virtual long TaxPayerID { get; set; }
        public virtual long TaxPayerTypeID { get; set; }



        /// <summary>
        /// AssessmentID (e.g 10009) for assessment and ServiceBillID (e.g 10007) for service bill
        /// </summary>
        public virtual long ReferenceID { get; set; }
        public virtual bool IsPaymentSuccessful { get; set; }

        //NO RIN Capture objects
        public virtual string TaxPayerType { get; set; }
        public virtual string Email { get; set; }
        public virtual string EconomicActivity { get; set; }
        public virtual string Address { get; set; }
        public virtual string RevenueStream { get; set; }
        public virtual string RevenueSubStream { get; set; }
        public virtual string OtherInformation { get; set; }


        //Inner Bill Details Additional Objects

        public virtual string TemplateType { get; set; }
        public virtual DateTime ReferenceDate { get; set; }
        public virtual string SettlementStatusName { get; set; }
        public virtual string ReferenceNotes { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual decimal TotalOutstandingAmount { get; set; }
        public virtual decimal TotalAmountToPay { get; set; }
        public virtual int SettlementMethod { get; set; }
        public virtual string AddNotes { get; set; }
        //public virtual IEnumerable<RefRule> RefRules { get; set; }

    }





}
