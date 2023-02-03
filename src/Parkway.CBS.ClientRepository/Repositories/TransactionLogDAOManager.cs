using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class TransactionLogDAOManager : Repository<TransactionLog>, ITransactionLogDAOManager
    {
        public TransactionLogDAOManager(IUoW uow) : base(uow)
        {

        }


        /// <summary>
        /// Create TransactionLog for Reference Data Records
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="revenueHead"></param>
        public void CreateBatchInvoiceTransactionLog(long batchId, RevenueHead revenueHead)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_TransactionLog (Invoice_Id, AmountPaid, PaymentDate, PaymentReference, Status, TaxEntity_Id, TaxEntityCategory_Id, InvoiceNumber, TypeID, MDA_Id, RevenueHead_Id, CreatedAtUtc, UpdatedAtUtc)" +
                    $" SELECT  I.Id, RDRInv.InvoiceAmount, :dateSaved, RDRInv.InvoiceNumber, :invStatus, RDRInv.TaxEntity_Id, RDRInv.TaxEntityCategory_Id, RDRInv.InvoiceNumber, :typeId, :mdaId, :revId, :dateSaved, :dateSaved" +
                    $" FROM Parkway_CBS_Core_ReferenceDataRecordsInvoice as RDRInv INNER JOIN Parkway_CBS_Core_Invoice as I ON I.InvoiceNumber = RDRInv.InvoiceNumber AND I.TaxPayer_Id =  RDRInv.TaxEntity_Id WHERE RDRInv.ReferenceDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", revenueHead.Mda.Id);
                query.SetParameter("revId", revenueHead.Id);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("invStatus", PaymentStatus.Pending.ToString());
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();

            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create TransactionLog for NAGIS Old Invoice Migration
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateNAGISInvoiceTransactionLog(long batchId)
        {
            try
            {
                var queryText = $" INSERT INTO Parkway_CBS_Core_TransactionLog (Invoice_Id, AmountPaid, PaymentDate, PaymentReference, Status, TaxEntity_Id, TaxEntityCategory_Id, InvoiceNumber, TypeID, MDA_Id, RevenueHead_Id, PaymentMethodId, PaymentProvider," +
                                $" TotalAmountPaid, Fee, InvoiceItem_Id, CreatedAtUtc, UpdatedAtUtc, Channel) SELECT" +
                                $" invItems.Invoice_Id, invItems.UnitAmount, :dateSaved, invItems.InvoiceNumber, :status, invItems.TaxEntity_Id, invItems.TaxEntityCategory_Id, invItems.InvoiceNumber, :typeId, invItems.MDA_Id, invItems.RevenueHead_Id, :PaymentMethodId," +
                                $" :PaymentProvider, :TotalAmountPaid, :Fee, invItems.Id, :dateSaved, :dateSaved, :PaymentMethodId" +
                                $" FROM Parkway_CBS_Core_NagisOldInvoiceSummary NagSum " +
                                $" INNER JOIN Parkway_CBS_Core_InvoiceItems invItems ON NagSum.InvoiceNumber = invItems.InvoiceNumber AND NagSum.TaxEntity_Id = invItems.TaxEntity_Id" +
                                $" WHERE NagSum.NagisDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("status", "Pending");
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("PaymentMethodId", 0);
                query.SetParameter("PaymentProvider", 0);
                query.SetParameter("TotalAmountPaid", 0);
                query.SetParameter("Fee", 0);

                query.ExecuteUpdate();

            }
            catch (Exception exception)
            {
                throw;
            }
        }


        public dynamic GetTotalAmountReceivedForNiger(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id != 29";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetTotalAmountReceivedNasarawaEx(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND MDA_Id = 3 AND RevenueHead_Id != 86";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetTotalAmountReceivedNasarawaOnly(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 377";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetTotalAmountReceivedNasarawa(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND MDA_Id IN (62, 61,18,24,59,58,57,44,22,25,4,8,9,15,16,17,20,21,23,27,30,34,35,40,37,48,56,55,54,53,52,51,50,49,47,46,43,42,41,39,38,36,33,32,31,29,28,26,19,14,13,12,11,2,68)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedGeneric(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = :revId";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("revId", revenueHeadId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceived(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (650,651,652,653,727)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if(bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }
                    
                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                retObj.QueryString = query.QueryString;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedSOHT(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (657, 659, 378)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedPG(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (656,44,55,655,728)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedAG1(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 98";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region MDA and one Rev All channels

        public dynamic GetTotalAmountReceivedMDAAndOneRevAllChannel(int mdaId, int revId, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND RevenueHead_Id = :revId AND TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("revId", revId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueMDAAndOneRevAllChannel(int mdaId, int revId, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND RevenueHead_Id = :revId AND TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("revId", revId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        #endregion


        #region MDA and one Rev 

        public dynamic GetTotalAmountReceivedMDAAndOneRev(int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND RevenueHead_Id = :revId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("revId", revId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueMDAAndOneRev(int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND RevenueHead_Id = :revId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("revId", revId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion



        #region MDA and one Rev Less settlement fee

        public dynamic GetTotalAmountReceivedMDAAndOneRevLessSettlementFee(int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(SettlementAmount) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND RevenueHead_Id = :revId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("revId", revId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region MDA and Rev list



        public dynamic GetTotalAmountReceivedRevListAllChannels(string revList, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN ({revList})";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                retObj.QueryString = query.QueryString;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueRevListAllChannels(string revList, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN ({revList})";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        
        public dynamic GetTotalAmountReceivedMDAAndRevList(int mdaId, string revList, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN ({revList})";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                retObj.QueryString = query.QueryString;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueMDAAndRevList(int mdaId, string revList, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN ({revList})";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion



        public dynamic GetTotalAmountReceivedMDAAndRevList(int mdaId, string revList, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN ({revList})";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                retObj.QueryString = query.QueryString;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueMDAAndRevList(int mdaId, string revList, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN ({revList})";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }






        #region CHITHUB

        public dynamic GetTotalAmountReceivedForChitHub(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (3154, 3153, 3152, 3151, 3150, 3159, 3158, 3157, 3156, 3155, 3164, 3163, 3162, 3161, 3160, 3169, 3168, 3167, 3166, 3165, 3174, 3173, 3172, 3171, 3170, 3179, 3178, 3177, 3176, 3175, 3184, 3183, 3182, 3181, 3180, 3189, 3188, 3187, 3186, 3185, 3194, 3193, 3192, 3191, 3190, 3199, 3198, 3197, 3196, 3195, 3204, 3203, 3202, 3201, 3200, 3209, 3208, 3207, 3206, 3205, 3214, 3213, 3212, 3211, 3210, 3219, 3218, 3217, 3216, 3215, 3224, 3223, 3222, 3221, 3220, 3229, 3228, 3227, 3226, 3225, 3234, 3233, 3132, 3231, 3230, 3239, 3238, 3237, 3236, 3235, 3244, 3243, 3242, 3241, 3240, 3249, 3248, 3247, 3246, 3245, 3254, 3253, 3252, 3251, 3250, 3259, 3258, 3257, 3256, 3255, 3264, 3263, 3262, 3261, 3260, 3269, 3268, 3267, 3266, 3265, 3274, 3273, 3272, 3271, 3270, 3279, 3278, 3277, 3276, 3275, 3284, 3283, 3282, 3281, 3280, 3289, 3288, 3287, 3286, 3285, 3294, 3293, 3292, 3291, 3290, 3299, 3298, 3297, 3296, 3295, 3304, 3303, 3302, 3301, 3300, 3309, 3308, 3307, 3306, 3305, 3314, 3313, 3312, 3311, 3310, 3319, 3318, 3317, 3316, 3315, 3321, 3320)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                retObj.QueryString = query.QueryString;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// who is the expert system Id here
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="provider"></param>
        /// <param name="channel"></param>
        /// <param name="date"></param>
        /// <param name="who"></param>
        public void SetSettledTransactionsToTrueForChitHub(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (3154, 3153, 3152, 3151, 3150, 3159, 3158, 3157, 3156, 3155, 3164, 3163, 3162, 3161, 3160, 3169, 3168, 3167, 3166, 3165, 3174, 3173, 3172, 3171, 3170, 3179, 3178, 3177, 3176, 3175, 3184, 3183, 3182, 3181, 3180, 3189, 3188, 3187, 3186, 3185, 3194, 3193, 3192, 3191, 3190, 3199, 3198, 3197, 3196, 3195, 3204, 3203, 3202, 3201, 3200, 3209, 3208, 3207, 3206, 3205, 3214, 3213, 3212, 3211, 3210, 3219, 3218, 3217, 3216, 3215, 3224, 3223, 3222, 3221, 3220, 3229, 3228, 3227, 3226, 3225, 3234, 3233, 3132, 3231, 3230, 3239, 3238, 3237, 3236, 3235, 3244, 3243, 3242, 3241, 3240, 3249, 3248, 3247, 3246, 3245, 3254, 3253, 3252, 3251, 3250, 3259, 3258, 3257, 3256, 3255, 3264, 3263, 3262, 3261, 3260, 3269, 3268, 3267, 3266, 3265, 3274, 3273, 3272, 3271, 3270, 3279, 3278, 3277, 3276, 3275, 3284, 3283, 3282, 3281, 3280, 3289, 3288, 3287, 3286, 3285, 3294, 3293, 3292, 3291, 3290, 3299, 3298, 3297, 3296, 3295, 3304, 3303, 3302, 3301, 3300, 3309, 3308, 3307, 3306, 3305, 3314, 3313, 3312, 3311, 3310, 3319, 3318, 3317, 3316, 3315, 3321, 3320)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        #endregion


        #region Niger all

        public dynamic GetTotalAmountReceivedAllMDA(int mdaId, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueAllMDA(int mdaId, PaymentProvider provider, DateTime date)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        public dynamic GetTotalAmountReceivedAllMDA(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueAllMDA(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion


        #region CEA128


        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedCEA128(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 128";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueCEA128(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 128";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }



        #endregion



        #region DASH 01

        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedDASH01(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (736, 737, 769)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueDASH01(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (736, 737, 769)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion


        #region DASH 02

        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedDASH02(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (735, 734, 733, 738, 739, 740, 770)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueDASH02(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (735, 734, 733, 738, 739, 740, 770)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion


        #region DASH 03

        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedDASH03(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (741, 135, 742, 743, 744, 766, 767, 768)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueDASH03(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (741, 135, 742, 743, 744, 766, 767, 768)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion


        #region DASH 04

        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedDASH04(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 772, 764, 765)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueDASH04(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 772, 764, 765)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion


        #region DASH 05

        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedDASH05(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (759, 760, 761, 762, 763, 771)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueDASH05(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (759, 760, 761, 762, 763, 771)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion



        #region NASARAWA BRAODCASTING SERVICE (MDA Id = 32) | NASARAWA STATE WATER BOARD (MDA Id = 35)

        public dynamic GetTotalAmountReceivedNasarawaAllMDA(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueNasarawaAllMDA(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion


        #region COLLEGE OF AGRIC – NON-PORTAL SERVICES (MDA Id = 10)

        public dynamic GetTotalAmountReceivedNasarawaCollAgr(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (662, 663, 664, 99, 665, 96, 95, 667, 668, 669, 670, 105, 671, 666, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetSettledTransactionsToTrueNasarawaCollAgr(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (662, 663, 664, 99, 665, 96, 95, 667, 668, 669, 670, 105, 671, 666, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }



        #endregion


        #region NASPOLY NEW PROTAL

        public dynamic GetTotalAmountReceivedNasPolyNew(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (522, 523, 524, 525, 518, 519, 520, 521, 514, 515, 516, 517, 512, 513, 508, 509, 510, 511, 504, 505, 506, 507, 500, 501, 502, 503, 496, 497, 498, 499, 342, 493, 494, 495, 339, 341, 343, 334, 335, 336, 337, 338, 329, 330, 331, 332, 333, 324, 325, 326, 319, 320, 321, 322, 323, 314, 315, 316, 317, 318, 309, 310, 311, 312, 313, 306, 307, 308, 301, 302, 303, 304, 305, 6, 5, 4)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetSettledTransactionsToTrueNasPolyNew(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (522, 523, 524, 525, 518, 519, 520, 521, 514, 515, 516, 517, 512, 513, 508, 509, 510, 511, 504, 505, 506, 507, 500, 501, 502, 503, 496, 497, 498, 499, 342, 493, 494, 495, 339, 341, 343, 334, 335, 336, 337, 338, 329, 330, 331, 332, 333, 324, 325, 326, 319, 320, 321, 322, 323, 314, 315, 316, 317, 318, 309, 310, 311, 312, 313, 306, 307, 308, 301, 302, 303, 304, 305, 6, 5, 4)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        #endregion


        #region NASPOLY CONSULT HND (MDA Id = 5)

        public dynamic GetTotalAmountReceivedNasarawa5(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetSettledTransactionsToTrueNasarawa5(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        #endregion


        #region NASPOLY CONSULT OND (MDA Id = 5)

        public dynamic GetTotalAmountReceivedNasarawa15(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetSettledTransactionsToTrueNasarawa15(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        #endregion


        #region MINISTRY OF TRADE AND INVESTMENT (MDA Id = 18)

        public dynamic GetTotalAmountReceivedNasarawa18(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (164, 166, 729, 730, 731, 732, 170, 176)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetSettledTransactionsToTrueNasarawa18(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (164, 166, 729, 730, 731, 732, 170, 176)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        #endregion



        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedAG2(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (660, 103, 104, 100, 101, 102, 93, 94, 97)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetTotalAmountReceivedSOH(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 658";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        public dynamic GetTotalAmountReceivedNasPoly(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetTotalAmountReceivedIDEC(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetTotalAmountReceivedIDECNIBSS(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                //query.AddEntity(typeof(decimal));
                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SetSettledTransactionsToTrueNiger(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id != 29";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }



        public void SetSettledTransactionsToTrueNasarawa(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND MDA_Id IN (62, 61,18,24,59,58,57,44,22,25,4,8,9,15,16,17,20,21,23,27,30,34,35,40,37,48,56,55,54,53,52,51,50,49,47,46,43,42,41,39,38,36,33,32,31,29,28,26,19,14,13,12,11,2,68)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }

        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetSettledTransactionsToTrueGeneric(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = :revId";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("revId", revenueHeadId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                
                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }



        public void SetSettledTransactionsToTrueNasarawaOnly(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 377";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        public void SetSettledTransactionsToTrueNasarawaEx(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND MDA_Id = 3 AND RevenueHead_Id != 86";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetSettledTransactionsToTrue(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (650,651,652,653,727)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }

        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetSettledTransactionsToTrueSOHT(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (657, 659, 378)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetSettledTransactionsToTruePG(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (656,44,55,655,728)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetSettledTransactionsToTrueAG1(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 98";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetSettledTransactionsToTrueAG2(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id IN (660, 103, 104, 100, 101, 102, 93, 94, 97)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        public void SetSettledTransactionsToTrueSOH(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND RevenueHead_Id = 658";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        /// <summary>
        /// Set confirmation for IPPIS direct assessment to true
        /// </summary>
        public void SetSettledTransactionsToTrueNasPoly(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate, Who = :who FROM Parkway_CBS_Core_TransactionLog tr WHERE MDA_Id = :mdaId AND TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("mdaId", mdaId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("who", who);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        public void SetSettledTransactionsToTrueIDEC(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }

        public void SetSettledTransactionsToTrueIDECNIBSS(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Settled = :trueVal, UpdatedAtUtc = :updateDate FROM Parkway_CBS_Core_TransactionLog tr WHERE TypeID != :typeId AND PaymentProvider = :provider AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { throw; }
        }


        #region ITEX 3 and 86

        public dynamic GetTotalAmountReceivedITEX3(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date)
        {
            try
            {
                var queryText = $" SELECT SUM(AmountPaid) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Core_TransactionLog WHERE TypeID != :typeId AND PaymentProvider = :provider AND Channel = :channel AND Settled = :settledVal AND cast (CreatedAtUtc as date) = :date AND MDA_Id = 3 AND RevenueHead_Id != 86";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("provider", (int)provider);
                query.SetParameter("channel", (int)channel);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", date.ToString("yyyy-MM-dd"));

                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
