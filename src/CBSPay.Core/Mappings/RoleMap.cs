using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBSPay.Core.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace CBSPay.Core.Mappings
{
    public class RoleMap : ClassMapping<Role>
    {
        public RoleMap()
        {

            Table("Role");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.Name, m => m.NotNullable(true));
           
        }
    }
}
