﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class RemitaPaymentNotification
    {
        /// <summary>
        /// Unique reference generated by Remita for every transaction.
        /// </summary>
        public string rrr { get; set; }
        public string channel { get; set; }
        public double amount { get; set; }
        public string transactiondate { get; set; }
        public string debitdate { get; set; }
        public string bank { get; set; }
        public string branch { get; set; }
        /// <summary>
        /// Unique Identifier for each service type on Remita. Distinct Service Ids will be provided for each service type.
        /// </summary>
        public string serviceTypeId { get; set; }
        public string dateRequested { get; set; }
        /// <summary>
        /// Unique Identifier field will be used to transmit the uid during payment notification post.
        /// </summary>
        public string orderRef { get; set; }
        public string payerName { get; set; }
        public string payerPhoneNumber { get; set; }
        public string payerEmail { get; set; }
        public string agencyCode { get; set; }
        public string revenueCode { get; set; }
        public List<CustomFieldData> customFieldData { get; set; }
    }

    public class CustomFieldData
    {
        public string DESCRIPTION { get; set; }
        public string COLVAL { get; set; }
    }
}