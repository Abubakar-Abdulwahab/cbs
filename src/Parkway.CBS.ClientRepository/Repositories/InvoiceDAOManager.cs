using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class InvoiceDAOManager : Repository<Invoice>, IInvoiceDAOManager
    {

        public InvoiceDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// create the invoice for IPPIS record
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateIPPISInvoices(long batchId, RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails)
        {
            try
            {
                var queryText = $"MERGE Parkway_CBS_Core_Invoice cbsInv USING Parkway_CBS_Core_IPPISBatchRecordsInvoice ipInv ON(ipInv.Id = 0) WHEN NOT MATCHED BY TARGET AND ipInv.IPPISBatch_Id = :batch_Id THEN INSERT (InvoiceNumber, MDA_Id, RevenueHead_Id, Amount, TaxPayer_Id, TaxPayerCategory_Id, ExpertSystemSettings_Id, InvoiceURL, CashflowInvoiceIdentifier, InvoiceModel, Status, Quantity, DueDate, CreatedAtUtc, UpdatedAtUtc, InvoiceDescription, InvoiceType, InvoiceTitle) VALUES(ipInv.InvoiceNumber, :mdaId, :revId, ipInv.InvoiceAmount, ipInv.TaxEntity_Id, ipInv.TaxEntityCategory_Id, :expSysId, ipInv.InvoiceURL, ipInv.CashflowInvoiceIdentifier, ipInv.InvoiceModel, :invStatus, :quantity, ipInv.DueDate, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, ipInv.InvoiceDescription, :invoiceType, :invoiceTitle)  OUTPUT ipInv.InvoiceNumber as InvoiceNumber, inserted.Id as Invoice_Id, :mdaId, :revId, ipInv.InvoiceAmount as UnitAmount, ipInv.TaxEntity_Id as TaxEntity_Id, ipInv.TaxEntityCategory_Id as TaxEntityCategory_Id, :quantity, ipInv.CashflowInvoiceIdentifier as InvoicingUniqueIdentifier, CURRENT_TIMESTAMP as CreatedAtUtc, CURRENT_TIMESTAMP as UpdatedAtUtc INTO Parkway_CBS_Core_InvoiceItems(InvoiceNumber, Invoice_Id, MDA_Id, RevenueHead_Id, UnitAmount, TaxEntity_Id, TaxEntityCategory_Id, Quantity, InvoicingUniqueIdentifier, CreatedAtUtc, UpdatedAtUtc);";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", revenueHeadDetails.MDAId);
                query.SetParameter("revId", revenueHeadDetails.RevenueHeadId);
                query.SetParameter("expSysId", revenueHeadDetails.ExpertSystemId);
                query.SetParameter("invoiceTitle", revenueHeadDetails.RevenueHeadNameAndCode);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("quantity", 1);
                query.SetParameter("invStatus", (int)InvoiceStatus.Unpaid);
                query.SetParameter("invoiceType", (int)InvoiceType.DirectAssessment);
                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="revenueHead"></param>
        public void CreateBatchInvoices(long batchId, RevenueHead revenueHead)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_Invoice (InvoiceNumber, MDA_Id, RevenueHead_Id, Amount, TaxPayer_Id, TaxPayerCategory_Id, ExpertSystemSettings_Id, InvoiceURL, CashflowInvoiceIdentifier, InvoiceModel, Status, DueDate, CreatedAtUtc, UpdatedAtUtc, InvoiceDescription, InvoiceType) SELECT  RDRInv.InvoiceNumber, :mdaId, :revId, RDRInv.InvoiceAmount, RDRInv.TaxEntity_Id, RDRInv.TaxEntityCategory_Id, :expSysId, RDRInv.InvoiceURL, RDRInv.CashflowInvoiceIdentifier, RDRInv.InvoiceModel, :invStatus, RDRInv.DueDate, :dateSaved, :dateSaved, RDRInv.InvoiceDescription, :invoiceType FROM Parkway_CBS_Core_ReferenceDataRecordsInvoice RDRInv WHERE RDRInv.ReferenceDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", revenueHead.Mda.Id);
                query.SetParameter("revId", revenueHead.Id);
                query.SetParameter("expSysId", revenueHead.Mda.ExpertSystemSettings.Id);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("invStatus", (int)InvoiceStatus.Unpaid);
                query.SetParameter("invoiceType", (int)InvoiceType.Standard);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();

            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateNAGISInvoices(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_Invoice (InvoiceNumber, MDA_Id, RevenueHead_Id, Amount, TaxPayer_Id, TaxPayerCategory_Id, ExpertSystemSettings_Id, InvoiceURL, CashflowInvoiceIdentifier, Status, ExternalRefNumber, DueDate, CreatedAtUtc, UpdatedAtUtc, InvoiceDescription, InvoiceType, InvoiceTitle) SELECT  NavInv.InvoiceNumber, NavInv.MDAId, NavInv.RevenueHead_Id, NavInv.AmountDue, NavInv.TaxEntity_Id, NavInv.TaxEntityCategory_Id, NavInv.ExpertSystemId, NavInv.InvoiceURL, NavInv.CashflowInvoiceIdentifier, NavInv.StatusId, NavInv.NagisInvoiceNumber, NavInv.DueDate, :dateSaved, :dateSaved, NavInv.InvoiceDescription, :invoiceType, NavInv.InvoiceDescription FROM Parkway_CBS_Core_NagisOldInvoiceSummary NavInv WHERE NavInv.NagisDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("invoiceType", (int)InvoiceType.Standard);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();

            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// create the Transaction Log for IPPIS invoices
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="revenueHeadDetails"></param>
        public void CreateIPPISInvoiceTransactionLog(long batchId)
        {
            try
            {

                var queryText = $"INSERT INTO Parkway_CBS_Core_TransactionLog (Invoice_Id, AmountPaid, PaymentDate, PaymentReference, Status, TaxEntity_Id, TaxEntityCategory_Id, InvoiceNumber, TypeID, MDA_Id, RevenueHead_Id, PaymentMethodId, PaymentProvider," +
                $" TotalAmountPaid, Fee, InvoiceItem_Id, CreatedAtUtc, UpdatedAtUtc, Channel) SELECT" +
                $" invItems.Invoice_Id, invItems.UnitAmount, :dateSaved, invItems.InvoiceNumber, :status, invItems.TaxEntity_Id, invItems.TaxEntityCategory_Id, invItems.InvoiceNumber, :typeId, invItems.MDA_Id, invItems.RevenueHead_Id, :PaymentMethodId," +
                $" :PaymentProvider, :TotalAmountPaid, :Fee, invItems.Id, :dateSaved, :dateSaved, :PaymentMethodId" +
                $" FROM Parkway_CBS_Core_IPPISBatchRecordsInvoice IPInv" +
                $" INNER JOIN Parkway_CBS_Core_InvoiceItems invItems ON IPInv.InvoiceNumber = invItems.InvoiceNumber" +
                $" WHERE IPInv.IPPISBatch_Id = :batch_Id";

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

        /// <summary>
        /// This checks through the invoice list and cancel those that have not been paid but have passed the due date
        /// </summary>
        /// <returns>string</returns>
        public string ProcessInvoiceCancellation()
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                DateTime dateToCurrentHour = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0);

                var queryText = $"UPDATE inv SET inv.status = :status, inv.UpdatedAtUtc = :dateUpdated FROM Parkway_CBS_Core_Invoice inv" +
                    $" WHERE inv.DueDate <= :dueDate AND inv.Status != :paidStatusId AND inv.Status != :writeOffStatusId";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("status", (int)InvoiceStatus.WriteOff);
                query.SetParameter("dateUpdated", DateTime.Now.ToLocalTime());
                query.SetParameter("dueDate", dateToCurrentHour);
                query.SetParameter("paidStatusId", (int)InvoiceStatus.Paid);
                query.SetParameter("writeOffStatusId", (int)InvoiceStatus.WriteOff);

                int affectedRecord = query.ExecuteUpdate();
                return $"Successful. {affectedRecord} processed.";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
