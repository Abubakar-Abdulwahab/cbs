using CBSPay.Core.Models;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Mappings
{
    public class EIRSPaymentRequestMap : ClassMapping<EIRSPaymentRequest>
    {
        public EIRSPaymentRequestMap()
        {
            Table("EIRSPaymentRequest");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.ReferenceNumber);
            Property(p => p.Description, m => m.Length(255));
            Property(p => p.TaxPayerName, m => m.Length(255));
            Property(p => p.PhoneNumber, m => m.Length(25));
            Property(p => p.PaymentIdentifier, m => m.Length(255));
            Property(p => p.TaxPayerTIN); 
            Property(p => p.TaxPayerRIN); 
            Property(p => p.TaxPayerID); 
            Property(p => p.TaxPayerTypeID);
            Property(p => p.ReferenceID);
            Property(p => p.IsPaymentSuccessful);
            Property(p => p.TotalAmountPaid);
            Property(p => p.TaxPayerType);
            Property(p => p.Email);
            Property(p => p.EconomicActivity);
            Property(p => p.Address);
            Property(p => p.RevenueStream);
            Property(p => p.RevenueSubStream);
            Property(p => p.OtherInformation);
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted, m => m.NotNullable(true));
            Bag(p => p.PaymentRequestItems, map =>
            {
                map.Inverse(true);
                map.Cascade(Cascade.All);
                map.Key(k => k.Column("EIRSPaymentRequestId"));
            }, rel => rel.OneToMany(a => a.Class(typeof(EIRSPaymentRequestItem))));

            //new fields 14-08-2018
            Property(p => p.TemplateType, m => m.Length(255));
            Property(p => p.SettlementStatusName, m => m.Length(255));
            Property(p => p.ReferenceNotes, m => m.Length(25));
            Property(p => p.AddNotes, m => m.Length(255));
            Property(p => p.ReferenceDate);
            Property(p => p.TotalAmount);
            Property(p => p.TotalOutstandingAmount);
            Property(p => p.TotalAmountToPay);
            Property(p => p.SettlementMethod);
        }
    }
}
