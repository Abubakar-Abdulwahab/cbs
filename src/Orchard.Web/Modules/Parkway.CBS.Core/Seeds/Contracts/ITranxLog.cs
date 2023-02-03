using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface ITranxLog : IDependency
    {
        void CorrectPaydirectBankCodeIssue();
    }
}
