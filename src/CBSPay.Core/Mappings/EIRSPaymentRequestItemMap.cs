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
    public class EIRSPaymentRequestItemMap : ClassMapping<EIRSPaymentRequestItem>
    {
        public EIRSPaymentRequestItemMap()
        {
            Table("EIRSPaymentRequestItem");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.ItemId);
            Property(p => p.ItemDescription, m => m.Length(255));
            Property(p => p.AmountPaid);
            Property(p => p.ItemAmount);
            Property(p => p.TotalAmountPaid);
            ManyToOne(p => p.PaymentRequest, map => map.Column("EIRSPaymentRequestId"));
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted);

            //new items 14-08-2018
            Property(p => p.RuleName, m => m.Length(255));
            Property(p => p.RuleItemRef, m => m.Length(255));
            Property(p => p.RuleItemID, m => m.Length(255));
            Property(p => p.RuleItemName, m => m.Length(255));
            Property(p => p.RuleComputation, m => m.Length(255));
            Property(p => p.TaxYear);
            Property(p => p.RefRuleID);
            Property(p => p.RuleAmount);
            Property(p => p.SettledAmount);
            Property(p => p.OutstandingAmount);
            Property(p => p.RuleAmountToPay);
            Property(p => p.RuleID);
        }
    }
}
