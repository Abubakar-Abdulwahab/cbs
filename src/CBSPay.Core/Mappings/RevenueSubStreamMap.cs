using CBSPay.Core.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace CBSPay.Core.Mappings
{
    public class RevenueSubStreamMap : ClassMapping<RevenueSubStream>
    {
        public RevenueSubStreamMap()
        {
            Table("RevenueSubStream");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.RevenueSubStreamID);
            Property(p => p.RevenueSubStreamName);
            //Property(p => p.RevenueStreamID);
            //Property(p => p.RevenueStreamName);
            Property(p => p.Active);
            Property(p => p.ActiveText);
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted, m => m.NotNullable(true));
        }
    }
}
