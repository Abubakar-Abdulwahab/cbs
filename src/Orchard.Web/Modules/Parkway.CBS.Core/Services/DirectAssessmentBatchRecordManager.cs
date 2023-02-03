using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Services.Contracts;
using Orchard.Data;
using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Payee;
using System.Data;
using Orchard.Logging;
using NHibernate.Linq;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using NHibernate;

namespace Parkway.CBS.Core.Services
{

    public class DirectAssessmentBatchRecordManager : BaseManager<DirectAssessmentBatchRecord>, IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord>
    {
        private readonly IRepository<DirectAssessmentBatchRecord> _repository;
        //private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public DirectAssessmentBatchRecordManager(IRepository<DirectAssessmentBatchRecord> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get DirectAssessmentBatchRecord
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>DirectAssessmentBatchRecord</returns>
        public DirectAssessmentBatchRecord Get(long batchRecordId)
        {
            return _transactionManager.GetSession().Get<DirectAssessmentBatchRecord>(batchRecordId);
        }
    }


    public class DirectAssessmentPayeeManager : BaseManager<DirectAssessmentPayeeRecord>, IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord>
    {
        private readonly IRepository<DirectAssessmentPayeeRecord> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public DirectAssessmentPayeeManager(IRepository<DirectAssessmentPayeeRecord> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the list of DirectAssessmentPayeeRecord for a particular DirectAssessmentBatchRecord
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>IList{DirectAssessmentPayeeRecord}</returns>
        public IList<DirectAssessmentPayeeRecord> GetRecords(Int64 batchId, int take, int skip)
        {
            return _transactionManager.GetSession().QueryOver<DirectAssessmentPayeeRecord>().Where(x => x.DirectAssessmentBatchRecord == new DirectAssessmentBatchRecord { Id = batchId }).Skip(skip).Take(take).List<DirectAssessmentPayeeRecord>();
        }


        public void Delete(DirectAssessmentBatchRecord record)
        {
            using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
            {
                using (var tranx = session.BeginTransaction())
                {
                    try
                    {
                        var query = session.CreateQuery("Delete from " + typeof(DirectAssessmentPayeeRecord)+ " where DirectAssessmentBatchRecord_Id = :id");
                        query.SetParameter("id", record.Id);
                        query.ExecuteUpdate();
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Could not delete batch record object ", record.Id));
                        tranx.Rollback();
                        throw;
                    }
                }
            }
            Logger.Error(string.Format("Records have been deleted {0}", record.Id));
        }
        

        public ReceiptObj GetPayeReceipts(string phoneNumber, string receiptNumber, ReceiptStatus status, DateTime startDate, DateTime endDate, int skip, int take, bool queryForCount = false)
        {
            Logger.Information(string.Format("Infor {0} {1} {2} {3} {4} {5} {6} \\n", phoneNumber, receiptNumber, status.ToString(), startDate.ToString(), endDate.ToString(), skip, take));

            List<ReceiptItems> records = new List<ReceiptItems> { };
            IEnumerable<ReceiptItems> receiptFuture = null;
            IFutureValue<int> futureCount = null;
            try
            {
                if (startDate == null || endDate == null)
                {
                    startDate = new DateTime(startDate.Year, startDate.Month, 1);
                    endDate = new DateTime(endDate.Year, endDate.Month, 1);
                }
                bool paid = false;
                var session = _transactionManager.GetSession();

                if (status == ReceiptStatus.All)
                {
                    if (!string.IsNullOrEmpty(receiptNumber))
                    {
                        receiptFuture = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.ReceiptNumber == receiptNumber) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).Skip(skip).Take(take)
                            .Select(r => new ReceiptItems() { PayerName = r.DirectAssessmentBatchRecord.TaxEntity.Recipient, AnnualEarnings = r.GrossAnnual, Exemptions = r.Exemptions, PayerId = r.DirectAssessmentBatchRecord.TaxEntity.Id, TaxValue = r.IncomeTaxPerMonth, PaymentStatus = r.DirectAssessmentBatchRecord.PaymentStatus, ReceiptNumber = r.ReceiptNumber, Month = r.Month, Year = r.Year }).ToFuture();

                        if (queryForCount)
                        {
                            futureCount = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.ReceiptNumber == receiptNumber) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).GroupBy(r => r.PhoneNumber).Select(r => r.Count()).ToFutureValue<int>();
                        }                      
                    }
                    else
                    {
                        receiptFuture = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).Skip(skip).Take(take)
                            .Select(r => new ReceiptItems() { PayerName = r.DirectAssessmentBatchRecord.TaxEntity.Recipient, AnnualEarnings = r.GrossAnnual, Exemptions = r.Exemptions, PayerId = r.DirectAssessmentBatchRecord.TaxEntity.Id, TaxValue = r.IncomeTaxPerMonth, PaymentStatus = r.DirectAssessmentBatchRecord.PaymentStatus, ReceiptNumber = r.ReceiptNumber, Month = r.Month, Year = r.Year }).ToFuture();

                        if (queryForCount)
                        {
                            futureCount = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).GroupBy(r => r.PhoneNumber).Select(r => r.Count()).ToFutureValue<int>();
                        }
                    }
                }
                else
                {
                    if (status == ReceiptStatus.Paid) { paid = true; }
                    if (!string.IsNullOrEmpty(receiptNumber))
                    {
                        receiptFuture = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        (r.DirectAssessmentBatchRecord.PaymentStatus == paid) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.ReceiptNumber == receiptNumber) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).Skip(skip).Take(take)
                            .Select(r => new ReceiptItems() { PayerName = r.DirectAssessmentBatchRecord.TaxEntity.Recipient, AnnualEarnings = r.GrossAnnual, Exemptions = r.Exemptions, PayerId = r.DirectAssessmentBatchRecord.TaxEntity.Id, TaxValue = r.IncomeTaxPerMonth, PaymentStatus = r.DirectAssessmentBatchRecord.PaymentStatus, ReceiptNumber = r.ReceiptNumber, Month = r.Month, Year = r.Year }).ToFuture();

                        if (queryForCount)
                        {
                            futureCount = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        (r.DirectAssessmentBatchRecord.PaymentStatus == paid) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.ReceiptNumber == receiptNumber) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).GroupBy(r => r.PhoneNumber).Select(r => r.Count()).ToFutureValue<int>();
                        }
                    }
                    else
                    {
                        receiptFuture = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        (r.DirectAssessmentBatchRecord.PaymentStatus == paid) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).Skip(skip).Take(take)
                            .Select(r => new ReceiptItems() { PayerName = r.DirectAssessmentBatchRecord.TaxEntity.Recipient, AnnualEarnings = r.GrossAnnual, Exemptions = r.Exemptions, PayerId = r.DirectAssessmentBatchRecord.TaxEntity.Id, TaxValue = r.IncomeTaxPerMonth, PaymentStatus = r.DirectAssessmentBatchRecord.PaymentStatus, ReceiptNumber = r.ReceiptNumber, Month = r.Month, Year = r.Year }).ToFuture();

                        if (queryForCount)
                        {
                            futureCount = session.Query<DirectAssessmentPayeeRecord>()
                            .Where(r => (r.PhoneNumber == phoneNumber) &&
                                        (r.DirectAssessmentBatchRecord.PaymentStatus == paid) &&
                                        ((r.AssessmentDate >= startDate) && (r.AssessmentDate <= endDate)) &&
                                        (r.HasErrors == false) &&
                                        (r.DirectAssessmentBatchRecord.InvoiceConfirmed == true)
                                  ).GroupBy(r => r.PhoneNumber).Select(r => r.Count()).ToFutureValue<int>();
                        }
                    }
                }
                var dataSize = futureCount != null ? futureCount.Value : 0;

                double pageSize = ((double)dataSize / (double)take);
                int pages = 0;

                if (pageSize < 1 && dataSize >= 1) { pages = 1; }
                else { pages = (int)Math.Ceiling(pageSize); }

                return new ReceiptObj { ReceiptItems = receiptFuture.ToList(), PageSize = pages };
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); throw; }
        }


        public void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, DirectAssessmentBatchRecord record, TaxEntity entity)
        {
            Logger.Information("Saving direct assessment payee records for batch id " + record.Id);
            try
            {
                PayeeAssessmentLineRecordModel itemToSave = null;
                using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
                using (var tranx = session.BeginTransaction())
                {
                    session.SetBatchSize(payees.Count());
                    try
                    {
                        foreach (var item in payees)
                        {
                            DirectAssessmentPayeeRecord newRecord = new DirectAssessmentPayeeRecord
                            {
                                HasErrors = item.HasError,
                                IncomeTaxPerMonth = item.PayeeBreakDown.TaxStringValue,
                                IncomeTaxPerMonthValue = item.PayeeBreakDown.Tax,
                                TIN = item.TaxPayerTIN.Value,
                                Address = item.Address.Value,
                                Year = item.Year.StringValue,
                                Month = item.Month.StringValue,
                                Email = item.Email.Value,
                                PhoneNumber = item.Phone.Value,
                                LGA = item.LGA.Value,
                                DirectAssessmentBatchRecord = record,
                                Exemptions = item.Exemptions.StringValue,
                                GrossAnnual = item.GrossAnnualEarnings.StringValue,
                                ErrorMessages = item.ErrorMessages,
                                PayeeName = item.TaxPayerName.Value,
                                AssessmentDate = item.AssessmentDate,
                            };
                            itemToSave = item;
                            session.Insert(newRecord);
                        }
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        var message = string.Format("Could not save object {0} {1}", Utilities.Util.SimpleDump(itemToSave), exception.Message);
                        Logger.Error(exception, message);
                        tranx.Rollback();
                        throw new Exception();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, Int64 batchRecordId, TaxEntity entity)
        {
            Logger.Information("Saving direct assessment payee records for batch id " + batchRecordId);
            //save entities into temp table
            int chunkSize = 500000;
            var dataSize = payees.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;
            //var startTime = Stopwatch.StartNew();
            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(DirectAssessmentPayeeRecord).Name);
                    dataTable.Columns.Add(new DataColumn("GrossAnnual", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Exemptions", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("TIN", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("IncomeTaxPerMonth", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("IncomeTaxPerMonthValue", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("Month", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Year", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Email", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("PayeeName", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Address", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("LGA", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("DirectAssessmentBatchRecord_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("HasErrors", typeof(bool)));
                    dataTable.Columns.Add(new DataColumn("ErrorMessages", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("AssessmentDate", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    payees.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["GrossAnnual"] = x.GrossAnnualEarnings.StringValue;
                        row["Exemptions"] = x.Exemptions.StringValue;
                        row["TIN"] = x.TaxPayerTIN.Value;
                        row["IncomeTaxPerMonth"] = x.PayeeBreakDown.TaxStringValue;
                        row["IncomeTaxPerMonthValue"] = x.PayeeBreakDown.Tax;
                        row["Month"] = x.Month.StringValue;
                        row["Year"] = x.Year.StringValue;
                        row["Email"] = x.Email.Value;
                        row["PhoneNumber"] = x.Phone.Value;
                        row["PayeeName"] = x.TaxPayerName.Value;
                        row["Address"] = x.Address.Value;
                        row["LGA"] = x.LGA.Value;
                        row["HasErrors"] = x.HasError;
                        row["ErrorMessages"] = x.ErrorMessages;
                        row["DirectAssessmentBatchRecord_Id"] = batchRecordId;
                        row["AssessmentDate"] = x.AssessmentDate.HasValue? (object)x.AssessmentDate.Value: DBNull.Value;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    Logger.Information(string.Format("Insertion for direct assessment batch payee records  has started Size: {0} Skip: {1}", dataSize, skip));

                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(DirectAssessmentPayeeRecord).Name))
                    { throw new Exception("Error saving excel file details for batch Id " + batchRecordId); }

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message); throw;
            }            
            Logger.Information(string.Format("SIZE: {0}", dataSize));
        }
    }    
}