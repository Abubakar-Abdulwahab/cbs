using CBSPay.Core.Entities;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Models;
using CBSPay.Core.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Services
{
    public class AdminService : IAdminService
    {
        //private readonly IBaseRepository<EIRSPaymentRequest> _eirsPaymentRequestRepo;
        private readonly IBaseRepository<PaymentHistory> _paymentHistoryRepo;
        private readonly IBaseRepository<PaymentHistoryItem> _paymentHistoryItemRepo;
        private ILog Logger { get { return log4net.LogManager.GetLogger("CBSPay"); } }
        private readonly DateTime todaysDate = DateTime.Today.Date;
        private readonly DateTime dateOneWeekAgo = DateTime.Today.Date.AddDays(-7);

        public AdminService()
        {
            //_eirsPaymentRequestRepo = new Repository<EIRSPaymentRequest>();
            _paymentHistoryRepo = new Repository<PaymentHistory>();
            _paymentHistoryItemRepo = new Repository<PaymentHistoryItem>();
        }

        /// <summary>
        /// Get the sum of amount payable or saved FOR Bill (assesment and service bill) Settlement for today from PaymentHistory table
        /// </summary>
        /// <returns>the total amount in decimal or null if there is an error</returns>
        public decimal? GetTodayBillSettlementAmount()
        {
            try
            {
                Logger.Debug($"About to get total amount of bill settlement for today at {DateTime.Now.Date}");
                var amt = _paymentHistoryRepo.Fetch(x => x.DateCreated.Date == DateTime.Today.Date && x.ReferenceNumber != null && x.ReferenceNumber.Trim() != "").ToList();
                if (amt != null)
                {
                    var amtSum = amt.Sum(y => y.TotalAmountPaid);
                    Logger.Debug($"The sum at {DateTime.Now.TimeOfDay} is {amtSum}");
                    return amtSum;
                }
                Logger.Debug("No records returned for today");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully get today's bill settlement amount");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Get the sum of amount payable or saved for all Pay On Account for today from PaymentRequest table
        /// </summary>
        /// <returns>the total amount in decimal or null if there is an error</returns>
        public decimal? GetTodayPOAAmount()
        {
            try
            {
                Logger.Debug($"About to get total amount of pay on account for today at {DateTime.Now.Date}");
                var amt = _paymentHistoryRepo.Fetch(x => x.DateCreated.Date == DateTime.Today.Date && (x.ReferenceNumber == null || x.ReferenceNumber.Trim() == "")).ToList();
                if (amt != null)
                {
                    var amtSum = amt.Sum(y => y.TotalAmountPaid);
                    Logger.Debug($"The sum at {DateTime.Now.TimeOfDay} is {amtSum}");
                    return amtSum;
                }

                Logger.Debug("No records returned for today");
                return null;
                
            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully get today's pay on account amount");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Get the total sum of amount payable or saved for today from PaymentRequest table
        /// </summary>
        /// <returns>the total amount in decimal or null if there is an error</returns>
        public decimal? GetTodaysTotalTransaction()
        {
            try
            {
                Logger.Debug($"About to get total amount of transaction for today at {DateTime.Now.Date}");
                var amt = _paymentHistoryRepo.Fetch(x => x.DateCreated.Date == DateTime.Today.Date).ToList();
                if (amt != null)
                {
                    var amtSum = amt.Sum(y => y.TotalAmountPaid);
                    Logger.Debug($"The sum at {DateTime.Now.TimeOfDay} is {amtSum}");
                    return amtSum;
                }
                Logger.Debug("No records returned for today");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully get today's total transaction amount");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Returns payment transaction for the last on week, starting from sunday
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WeeklyPaymentTransaction> GetPaymentTransactionDetailsForOneWeek()
        {
            try
            {
                Logger.Debug($"Trying to fetch transaction detals for the past one week, today being {DateTime.Today.Date}");

                //var billSettlementRecord = _eirsPaymentRequestRepo.Fetch(x => dateOneWeekAgo.Date <= x.DateCreated.Date && x.DateCreated.Date <= todaysDate && string.IsNullOrWhiteSpace(x.TaxPayerRIN)).Select(x => x.TotalAmountToPay);

                //get list of amount based on category and group them by day of week

                var weekBillSettlement = GetThisWeekBillSettlementAmount();
                var weekPOA = GetThisWeekPOAAmount();
                var weekTotal = GetThisWeekTotalTransaction();

                
                var billSettlementRecords = weekBillSettlement?.ToLookup(rec => rec.DateCreated.DayOfWeek, rec => rec.TotalAmountPaid);
                var poaSettlementRecords = weekPOA?.ToLookup(rec => rec.DateCreated.DayOfWeek, rec => rec.TotalAmountPaid);
                var totalSettlementRecords = weekTotal?.ToLookup(rec => rec.DateCreated.DayOfWeek, rec => rec.TotalAmountPaid);

                //now split the records based on the day of week

                //sunday p.s - it returns null where there is no record
                var billRecordsForSunday = billSettlementRecords[DayOfWeek.Sunday].Any() ? billSettlementRecords[DayOfWeek.Sunday].Sum() : 0;
                var poaRecordsForSunday = poaSettlementRecords[DayOfWeek.Sunday].Any() ? poaSettlementRecords[DayOfWeek.Sunday].Sum() : 0;
                var totalRecordsForSunday = totalSettlementRecords[DayOfWeek.Sunday].Any() ? totalSettlementRecords[DayOfWeek.Sunday].Sum() : 0;

                var billRecordsForMonday = billSettlementRecords[DayOfWeek.Monday].Any() ? billSettlementRecords[DayOfWeek.Monday].Sum() : 0;
                var poaRecordsForMonday = poaSettlementRecords[DayOfWeek.Monday].Any() ? poaSettlementRecords[DayOfWeek.Monday].Sum() : 0;
                var totalRecordsForMonday = totalSettlementRecords[DayOfWeek.Monday].Any() ? totalSettlementRecords[DayOfWeek.Monday].Sum() : 0;

                var billRecordsForTuesday = billSettlementRecords[DayOfWeek.Tuesday].Any() ? billSettlementRecords[DayOfWeek.Tuesday].Sum() : 0;
                var poaRecordsForTuesday = poaSettlementRecords[DayOfWeek.Tuesday].Any() ? poaSettlementRecords[DayOfWeek.Tuesday].Sum() : 0;
                var totalRecordsForTuesday = totalSettlementRecords[DayOfWeek.Tuesday].Any() ? totalSettlementRecords[DayOfWeek.Tuesday].Sum() : 0;

                var billRecordsForWednesday = billSettlementRecords[DayOfWeek.Wednesday].Any() ? billSettlementRecords[DayOfWeek.Wednesday].Sum() : 0;
                var poaRecordsForWednesday = poaSettlementRecords[DayOfWeek.Wednesday].Any() ? poaSettlementRecords[DayOfWeek.Wednesday].Sum() : 0;
                var totalRecordsForWednesday = totalSettlementRecords[DayOfWeek.Wednesday].Any() ? totalSettlementRecords[DayOfWeek.Wednesday].Sum() : 0;

                var billRecordsForThursday = billSettlementRecords[DayOfWeek.Thursday].Any() ? billSettlementRecords[DayOfWeek.Thursday].Sum() : 0;
                var poaRecordsForThursday = poaSettlementRecords[DayOfWeek.Thursday].Any() ? poaSettlementRecords[DayOfWeek.Thursday].Sum() : 0;
                var totalRecordsForThursday = totalSettlementRecords[DayOfWeek.Thursday].Any() ? totalSettlementRecords[DayOfWeek.Thursday].Sum() : 0;

                var billRecordsForFriday = billSettlementRecords[DayOfWeek.Friday].Any() ? billSettlementRecords[DayOfWeek.Friday].Sum() : 0;
                var poaRecordsForFriday = poaSettlementRecords[DayOfWeek.Friday].Any() ? poaSettlementRecords[DayOfWeek.Friday].Sum() : 0;
                var totalRecordsForFriday = totalSettlementRecords[DayOfWeek.Friday].Any() ? totalSettlementRecords[DayOfWeek.Friday].Sum() : 0;

                var billRecordsForSaturday = billSettlementRecords[DayOfWeek.Saturday].Any() ? billSettlementRecords[DayOfWeek.Saturday].Sum() : 0;
                var poaRecordsForSaturday = poaSettlementRecords[DayOfWeek.Saturday].Any() ? poaSettlementRecords[DayOfWeek.Saturday].Sum() : 0;
                var totalRecordsForSaturday = totalSettlementRecords[DayOfWeek.Saturday].Any() ? totalSettlementRecords[DayOfWeek.Saturday].Sum() : 0;

                var thisWeekPaymentTransaction = new List<WeeklyPaymentTransaction>
                {
                    new WeeklyPaymentTransaction { DayOfWeek = DayOfWeek.Sunday, BillSettlementAmount = billRecordsForSunday, POAAmount = poaRecordsForSunday, TotalAmount = totalRecordsForSunday },
                    new WeeklyPaymentTransaction { DayOfWeek = DayOfWeek.Monday, BillSettlementAmount = billRecordsForMonday, POAAmount = poaRecordsForMonday, TotalAmount = totalRecordsForMonday },
                    new WeeklyPaymentTransaction { DayOfWeek = DayOfWeek.Tuesday, BillSettlementAmount = billRecordsForTuesday, POAAmount = poaRecordsForTuesday, TotalAmount = totalRecordsForTuesday },
                    new WeeklyPaymentTransaction { DayOfWeek = DayOfWeek.Wednesday, BillSettlementAmount = billRecordsForWednesday, POAAmount = poaRecordsForWednesday, TotalAmount = totalRecordsForWednesday },
                    new WeeklyPaymentTransaction { DayOfWeek = DayOfWeek.Thursday, BillSettlementAmount = billRecordsForThursday, POAAmount = poaRecordsForThursday, TotalAmount = totalRecordsForThursday },
                    new WeeklyPaymentTransaction { DayOfWeek = DayOfWeek.Friday, BillSettlementAmount = billRecordsForFriday, POAAmount = poaRecordsForFriday, TotalAmount = totalRecordsForFriday },
                    new WeeklyPaymentTransaction { DayOfWeek = DayOfWeek.Saturday, BillSettlementAmount = billRecordsForSaturday, POAAmount = poaRecordsForSaturday, TotalAmount = totalRecordsForSaturday }
                };

                Logger.Debug("successfully fetched PaymentTransactionDetails For One Week");
                return thisWeekPaymentTransaction;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured {ex.Message}, could not successfully fetch records");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        public List<PaymentHistory> GetThisWeekBillSettlementAmount()//private
        {
            try
            {
                Logger.Debug($"About to get total amount of bill settlement for this week ");
                var records = _paymentHistoryRepo.Fetch(x => dateOneWeekAgo.Date < x.DateCreated.Date && x.DateCreated.Date <= todaysDate && x.ReferenceNumber != null && x.ReferenceNumber.Trim() != "").ToList();
                return records;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully get this week's bill settlement amount");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

       
        public List<PaymentHistory> GetThisWeekPOAAmount()
        {
            try
            {
                Logger.Debug($"About to get total amount of pay on account for this week at {DateTime.Now.Date}");
                var records = _paymentHistoryRepo.Fetch(x => dateOneWeekAgo.Date < x.DateCreated.Date && x.DateCreated.Date <= todaysDate && (x.ReferenceNumber == null || x.ReferenceNumber.Trim() == "")).ToList();
                return records;

            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully get this weeks's pay on account amount");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Get the total sum of amount payable or saved for this week from PaymentRequest table
        /// </summary>
        /// <returns>the total amount in decimal or null if there is an error</returns>
        public List<PaymentHistory> GetThisWeekTotalTransaction()
        {
            try
            {
                Logger.Debug($"About to get total amount of transaction for today at {DateTime.Now.Date}");
                var records = _paymentHistoryRepo.Fetch(x => dateOneWeekAgo.Date < x.DateCreated.Date && x.DateCreated.Date <= todaysDate).ToList();
                return records;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully get this week's total transaction amount");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
    }
}
