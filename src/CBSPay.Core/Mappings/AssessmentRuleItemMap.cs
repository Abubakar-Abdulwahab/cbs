
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
    public class AssessmentRuleItemMap : ClassMapping<AssessmentRuleItem>
    {
        public AssessmentRuleItemMap()
        {
            Table("AssessmentRuleItem");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.AAIID);
            Property(p => p.AARID, m => m.Length(255));
            Property(p => p.AssessmentRuleID);
            Property(p => p.AssessmentRuleName);
            Property(p => p.AssessmentItemReferenceNo);
            Property(p => p.AssessmentItemID);
            Property(p => p.AssessmentItemName);
            Property(p => p.ComputationName);
            Property(p => p.PaymentStatusID);
            Property(p => p.PaymentStatusName);
            Property(p => p.TaxAmount); 
            Property(p => p.AmountPaid);
            Property(p => p.SettlementAmount);
            Property(p => p.PendingAmount);
            ManyToOne(p => p.AssessmentDetails, map => map.Column("AssessmentDetailsId"));
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted);
        }
    }
}
