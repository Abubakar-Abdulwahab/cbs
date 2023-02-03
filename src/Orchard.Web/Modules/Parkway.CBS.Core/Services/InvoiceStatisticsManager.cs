using System;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using NHibernate.Criterion;
using System.Linq;
using Parkway.CBS.Core.Events.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class InvoiceStatisticsManager : BaseManager<Stats>, IInvoiceStatisticsManager<Stats>
    {
        private readonly IRepository<Stats> _statsRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        private readonly IRevenueHeadStatisticsEventHandler _evnt;


        public InvoiceStatisticsManager(IRepository<Stats> statsRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices, IRevenueHeadStatisticsEventHandler evnt) : base(statsRepository, user, orchardServices)
        {
            _statsRepository = statsRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _evnt = evnt;
        }


        /// <summary>
        /// Get stat
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <param name="dueDate"></param>
        /// <returns>Stats | null</returns>
        public Stats GetStats(RevenueHead revenueHead, DateTime dueDate, TaxEntityCategory category)
        {
            string queryVal = string.Format("{0}|{1}|{2}", dueDate.ToString("dd'/'MM'/'yyyy"), revenueHead.Id, category.Id);
            return _statsRepository.Get(s => s.StatsQueryConcat == queryVal);
            //return _transactionManager.GetSession().CreateCriteria<Stats>().SetMaxResults(1)
            //                                        .Add(Restrictions.Eq("DueDate", dueDate))
            //                                        .Add(Restrictions.Eq("RevenueHead", revenueHead))
            //                                        .Add(Restrictions.Eq("TaxEntityCategory", category))
            //                                        .List<Stats>().FirstOrDefault();
        }


        /// <summary>
        /// Seed the convcats
        /// </summary>
        public void DoConcat()
        {
            var sts = _transactionManager.GetSession().QueryOver<Stats>().List<Stats>();
            foreach (var item in sts)
            {
                item.StatsQueryConcat = string.Format("{0}|{1}|{2}", item.DueDate.ToString("dd'/'MM'/'yyyy"), item.RevenueHead.Id, item.TaxEntityCategory.Id);
            }
        }


        public void DeleteAll()
        {
            var query = _transactionManager.GetSession().CreateQuery("Delete from " + typeof(Stats));
            query.ExecuteUpdate();
        }


        public void Populate()
        {
            var sess = _transactionManager.GetSession();
            var invoices = sess.QueryOver<Invoice>().List<Invoice>();

            foreach (var item in invoices)
            {
                _evnt.InvoiceAdded(new HelperModels.StatsContext { Invoice = item, MDA = item.Mda, RevenueHead = item.RevenueHead, TaxEntityCategory = item.TaxPayerCategory });
            }
        }


        public void Populate2()
        {
            try
            {
                var sess = _transactionManager.GetSession();
                //var logs = sess.QueryOver<TransactionLog>().Where(t => (t.Type == Models.Enums.PaymentType.Credit) && (!t.Reversed)).List<TransactionLog>();
                //var sum = logs.Sum(t => t.AmountPaid);

                var invoices = sess.QueryOver<Invoice>()
                    .Where(t => (t.Status != ((int)(Models.Enums.InvoiceStatus.Unpaid)))).List<Invoice>();
                foreach (var item in invoices)
                {
                    bool partPaid = true;
                    if (item.Status == (int)Models.Enums.InvoiceStatus.Paid) { partPaid = false; }
                    var amt = item.Payments.Where(p => (p.TypeID == (int)Models.Enums.PaymentType.Bill)).Sum(t => t.AmountPaid);

                    _evnt.InvoicePaymentNotification(new HelperModels.StatsContextUpdate { PartPayment = partPaid, AmountPaid = amt, Invoice = item, MDA = item.Mda, RevenueHead = item.RevenueHead, TaxEntityCategory = item.TaxPayerCategory });
                    sess.Flush();
                }

                //foreach (var item in logs)
                //{
                //    //_evnt.InvoicePaymentNotification(new HelperModels.StatsContext { Invoice = item, MDA = item.Mda, RevenueHead = item.RevenueHead, TaxEntityCategory = item.TaxPayerCategory });
                //    //_statsEventHandler.InvoicePaymentNotification(new StatsContextUpdate { AmountPaid = model.AmountPaid, Invoice = helperModel.Invoice, PartPayment = partPaid, MDA = new MDA { Id = helperModel.MDAId }, RevenueHead = new RevenueHead { Id = helperModel.RevenueHeadId }, TaxEntityCategory = new TaxEntityCategory { Id = helperModel.TaxCategoryId } });
                //    bool partPaid = false;
                //    if (item.AmountPaid < item.Invoice.Amount) { partPaid = true; }

                //    _evnt.InvoicePaymentNotification(new HelperModels.StatsContextUpdate { PartPayment = partPaid, AmountPaid = item.AmountPaid, Invoice = item.Invoice, MDA = item.MDA, RevenueHead = item.RevenueHead, TaxEntityCategory = item.TaxEntityCategory });
                //    sess.Flush();
                //}
            }
            catch (Exception exception)
            {

                throw;
            }
        }
    }
}