using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class WalletStatementCallLog : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual int WalletId { get; set; }

        public virtual int WalletIdentifierType { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }
    }
}