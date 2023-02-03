using System;
using System.Linq;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class PAYEReceipt : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Receipt Receipt { get; set; }


        public virtual IEnumerable<PAYEReceiptTransactionLog> PAYEReceiptTransactionLog { get; set; }


        public virtual IEnumerable<PAYEPaymentUtilization> PAYEPaymentUtilization { get; set; }

        /// <summary>
        /// 
        /// <see cref="Enums.PAYEReceiptUtilizationStatus"/>
        /// </summary>
        public virtual int UtilizationStatusId { get; set; }

        public decimal UtilizedAmount()
        {
            return PAYEPaymentUtilization.Sum(u => u.UtilizedAmount);
        }


        /// <summary>
        /// This is the total amount that was paid for with this receipt
        /// </summary>
        public decimal GetReceiptAmount()
        {
                return PAYEReceiptTransactionLog.Sum(t => t.TransactionLog.AmountPaid);
        }

    }
}