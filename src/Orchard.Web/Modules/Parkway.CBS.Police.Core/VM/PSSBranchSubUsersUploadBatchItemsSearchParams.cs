using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSBranchSubUsersUploadBatchItemsSearchParams
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public Int64 BatchId { get; set; }

        public bool PageData { get; set; }
    }
}