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
    public class AssessmentRuleMap : ClassMapping<AssessmentRule>
    {
        public AssessmentRuleMap()
        {
            Table("AssessmentRule");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.AARID);
            Property(p => p.AssessmentRuleID);
            Property(p => p.AssessmentRuleName);
            Property(p => p.AssessmentRuleAmount);
            Property(p => p.AssetID);
            Property(p => p.AssetTypeId);
            Property(p => p.AssetTypeName);
            Property(p => p.AssetRIN);
            Property(p => p.ProfileID);
            Property(p => p.ProfileDescription);
            Property(p => p.TaxYear);
            Property(p => p.SettledAmount);
            ManyToOne(p => p.AssessmentDetails, map => map.Column("AssessmentDetailsId"));
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted);
        }
    }
}
