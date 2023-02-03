using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class FailedWalletStatementRequestCallLog : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual int WalletId { get; set; }

        public virtual int WalletIdentifierType { get; set; }

        public virtual bool IsSuccessful { get; set; }

        public virtual int RetryCount { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }
    }
}