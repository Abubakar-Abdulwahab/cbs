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
    public class EconomicActivitiesMap : ClassMapping<EconomicActivities>
    {
        public EconomicActivitiesMap()
        {
            Table("EconomicActivities");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.EconomicActivitiesID);
            Property(p => p.EconomicActivitiesName);
            Property(p => p.TaxPayerTypeID);
            Property(p => p.TaxPayerTypeName);
            Property(p => p.Active);
            Property(p => p.ActiveText);
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted, m => m.NotNullable(true));
        }
    }
}
