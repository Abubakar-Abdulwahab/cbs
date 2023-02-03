using Parkway.CBS.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class RefDataDistinctGroupModel
    {
        /// <summary>
        /// collection of distinct ref data items
        /// </summary>
        public ConcurrentDictionary<string, RefDataAndCashflowDetails> DistinctItems { get; set; }


        /// <summary>
        /// collection of ref data items that are duplicate of what is already contained in the DistinctItems collection
        /// </summary>
        public ConcurrentStack<RefDataAndCashflowDetails> Duplicates { get; set; }
    }
}