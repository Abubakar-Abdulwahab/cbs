using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PAYEAPIBatchItemsRef : CBSBaseModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Item unique identifier
        /// </summary>
        public virtual int ItemNumber { get; set; }

        public virtual string Mac { get; set; }

        /// <summary>
        /// PAYEBatchItemsStaging id
        /// </summary>
        public virtual PAYEBatchItemsStaging PAYEBatchItemsStaging { get; set; }

        public virtual PAYEAPIRequest PAYEAPIRequest { get; set; }

        public virtual PAYEAPIBatchItemsPagesTracker PAYEAPIBatchItemsPagesTracker { get; set; }
    }
}