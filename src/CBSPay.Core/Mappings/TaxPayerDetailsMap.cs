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
    public class TaxPayerDetailsMap : ClassMapping<TaxPayerDetails>
    {
        public TaxPayerDetailsMap()
        {
            Table("TaxPayerDetails");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.TaxPayerID);
            Property(p => p.TaxPayerRIN, m => m.Length(255));
            Property(p => p.TaxPayerTypeID);
            Property(p => p.TaxPayerName);
            Property(p => p.TaxPayerMobileNumber);
            Property(p => p.TaxPayerTIN);
            Property(p => p.TaxPayerAddress);
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted);
        }
    }
}
