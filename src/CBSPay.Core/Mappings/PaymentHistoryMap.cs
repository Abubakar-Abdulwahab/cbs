using CBSPay.Core.Entities;
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
    public class PaymentHistoryMap : ClassMapping<PaymentHistory>
    {
        public PaymentHistoryMap()
        {
            Table("PaymentHistory");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.ReferenceNumber, m => m.Length(255));
            Property(p => p.PaymentChannel, m => m.Length(255));
            Property(p => p.UpdatedPaymentChannel, m => m.Length(255));
            Property(p => p.PaymentIdentifier, m => m.Length(255));
            Property(p => p.AmountPaid);
            Property(p => p.TotalAmountPaid);
            Property(p => p.ReferenceID);
            Property(p => p.ReferenceAmount);
            Property(p => p.TaxPayerID);
            Property(p => p.TaxPayerTypeID);
            Property(p => p.TaxPayerMobileNumber, m => m.Length(255)); 
            Property(p => p.TaxPayerName, m => m.Length(255));
            Property(p => p.TaxPayerTIN, m => m.Length(255)); 
            Property(p => p.TaxPayerRIN, m => m.Length(255));
            Property(p => p.PaymentDate, m => m.NotNullable(true));
            Property(p => p.IsRepeated);
            Property(p => p.PaymentLogId);
            Property(p => p.PaymentReference);
            Property(p => p.CustReference);
            Property(p => p.AlternateCustReference);
            Property(p => p.PaymentMethod);
            Property(p => p.ChannelName);
            Property(p => p.Location);
            Property(p => p.IsReversal);
            Property(p => p.SettlementDate);
            Property(p => p.InstitutionId);
            Property(p => p.InstitutionName);
            Property(p => p.BranchName);
            Property(p => p.BankName);
            Property(p => p.FeeName);
            Property(p => p.ReceiptNo);
            Property(p => p.PaymentCurrency);
            Property(p => p.OriginalPaymentLogId);
            Property(p => p.OriginalPaymentReference);
            Property(p => p.Teller);
            Property(p => p.TaxPayerType);
            Property(p => p.Email);
            Property(p => p.EconomicActivity);
            Property(p => p.Address);
            Property(p => p.RevenueStream);
            Property(p => p.RevenueSubStream);
            Property(p => p.OtherInformation);
            Property(p => p.IsCustomerDeposit);
            Property(p => p.IsSyncedWithEIRS);
            Property(p => p.Trials);
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted, m => m.NotNullable(true));
            Bag(p => p.PaymentItemsHistory, map =>
            {
                map.Inverse(true);
                map.Cascade(Cascade.All);
                map.Key(k => k.Column("PaymentHistoryId"));
            }, rel => rel.OneToMany(a => a.Class(typeof(PaymentHistoryItem))));
        }
    }
}
