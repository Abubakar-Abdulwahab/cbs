using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchItemsModel
    {
        /// <summary>
        ///  A unique identifier for each of the item to be processed
        /// </summary>
        public int ItemNumber { get; set; }

        /// <summary>
        /// The employee State TIN Id
        /// </summary>
        public string PayerId { get; set; }

        /// <summary>
        /// The Gross Annual earning for the Payee
        /// </summary>

        public decimal GrossAnnual { get; set; }

        /// <summary>
        /// The tax exemptions for the Payee
        /// </summary>
        public decimal Exemptions { get; set; }

        /// <summary>
        /// Income Tax Per Month
        /// </summary>
        public decimal IncomeTaxPerMonth { get; set; }

        /// <summary>
        /// The month to be paid for
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Year to be paid for
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// HMACSHA256 hash of the PayerId, IncomeTaxPerMonth (2 dp), Month and Year
        /// </summary>
        public string Mac { get; set; }
    }
}