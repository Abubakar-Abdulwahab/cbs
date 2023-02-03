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
    public class AssessmentDetailsMap : ClassMapping<AssessmentDetailsResult>
    {
        public AssessmentDetailsMap()
        {
            Table("AssessmentDetails");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.AssessmentID);
            Property(p => p.AssessmentNotes, m => m.Length(255));
            Property(p => p.AssessmentRefNo, m => m.Length(255));
            Property(p => p.PhoneNumber, m => m.Length(255));
            Property(p => p.AssessmentAmount);
            Property(p => p.AssessmentID);
            Property(p => p.TaxPayerID);
            Property(p => p.TaxPayerName, m => m.Length(255));
            Property(p => p.TaxPayerRIN, m => m.Length(255));
            Property(p => p.TaxPayerTypeName, m => m.Length(255));
            Property(p => p.TaxPayerTypeID);
            Property(p => p.SetlementAmount);
            Property(p => p.SettlementStatusID);
            Property(p => p.SettlementStatusName);
            Property(p => p.AssessmentDate);
            Property(p => p.Active);
            Property(p => p.ActiveText, m => m.Length(255));
            Property(p => p.SettlementDate);
            Property(p => p.SettlementDueDate);
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted, m => m.NotNullable(true));
            Bag(p => p.AssessmentRuleItems, map =>
            {
                map.Inverse(true);
                map.Cascade(Cascade.All);
                map.Key(k => k.Column("AssessmentDetailsId"));
            }, rel => rel.OneToMany(a => a.Class(typeof(AssessmentRuleItem))));
            Bag(p => p.AssessmentRules, map =>
            {
                map.Inverse(true);
                map.Cascade(Cascade.All);
                map.Key(k => k.Column("AssessmentDetailsId"));
            }, rel => rel.OneToMany(a => a.Class(typeof(AssessmentRule))));
        }
    }
}
