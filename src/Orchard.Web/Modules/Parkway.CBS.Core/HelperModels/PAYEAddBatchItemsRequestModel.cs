using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEAddBatchItemsRequestModel
    {
        /// <summary>
        /// Batch identifier for the PAYE items
        /// </summary>
        public string BatchIdentifier { get; set; }

        /// <summary>
        /// Page number for the request
        /// </summary>
        public int PageNumber { get; set; }

        public List<PAYEBatchItemsModel> PayeItems { get; set; }
    }
}