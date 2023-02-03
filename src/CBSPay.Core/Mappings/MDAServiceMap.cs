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
    public class MDAServiceMap : ClassMapping<MDAService>
    {
        public MDAServiceMap()
        {
            Table("MDAService");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.SBSID);
            Property(p => p.MDAServiceID);
            Property(p => p.MDAServiceName);
            Property(p => p.TaxYear);
            Property(p => p.ServiceAmount);
            Property(p => p.SettledAmount);
            ManyToOne(p => p.ServiceBill, map => map.Column("ServiceBillId"));
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted);
        }
    }
}
