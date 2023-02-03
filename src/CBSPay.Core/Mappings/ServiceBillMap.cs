using CBSPay.Core.Entities;
using CBSPay.Core.Models;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace CBSPay.Core.Mappings
{
    public class ServiceBillMap : ClassMapping<ServiceBillResult>
    {
        public ServiceBillMap()
        {
            Table("ServiceBill");
            Lazy(true);

            Id(p => p.Id, m => m.Generator(Generators.Identity));
            Property(p => p.ServiceBillID);
            Property(p => p.ServiceBillRefNo, m => m.Length(255));
            Property(p => p.Active);
            Property(p => p.ActiveText, m => m.Length(255));
            Property(p => p.PhoneNumber, m => m.Length(255));
            Property(p => p.ServiceBillAmount);
            Property(p => p.SetlementAmount);
            Property(p => p.SettlementDate);
            Property(p => p.SettlementDueDate);
            Property(p => p.SettlementStatusID);
            Property(p => p.SettlementStatusName, m => m.Length(255));
            Property(p => p.TaxPayerID);
            Property(p => p.TaxPayerName, m => m.Length(255));
            Property(p => p.TaxpayerRIN, m => m.Length(255));
            Property(p => p.ServiceBillDate);
            Property(p => p.DateCreated, m => m.NotNullable(true));
            Property(p => p.DateModified, m => m.NotNullable(true));
            Property(p => p.IsDeleted, m => m.NotNullable(true));
            Bag(p => p.ServiceBillItems, map =>
            {
                map.Inverse(true);
                map.Cascade(Cascade.All);
                map.Key(k => k.Column("ServiceBillId"));
            }, rel => rel.OneToMany(a => a.Class(typeof(ServiceBillItem))));
            Bag(p => p.MDAServiceRules, map =>
            {
                map.Inverse(true);
                map.Cascade(Cascade.All);
                map.Key(k => k.Column("ServiceBillId"));
            }, rel => rel.OneToMany(a => a.Class(typeof(MDAService))));
        }
    }
    
}
