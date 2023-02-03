using CBSPay.Core.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Mappings
{
    public class PaymentItemsHistoryMap :ClassMapping<PaymentHistoryItem>
    {
        public PaymentItemsHistoryMap()
        {
            Table("PaymentHistoryItem");
            Lazy(true);
            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.ItemId);
            Property(p => p.ItemDescription, m => m.Length(255));
            Property(p => p.ItemAmount);
            Property(p => p.AmountPaid);
            ManyToOne(p => p.PaymentHistory, map => map.Column("PaymentHistoryId"));
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted, m => m.NotNullable(true));
        }
    }
}
