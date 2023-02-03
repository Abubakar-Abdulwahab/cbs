using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface ITaxEntitySeeds : IDependency
    {
        bool GeneratePayerId();
        bool GenerateUnknownTaxEntity();
    }

    public interface IStatsSeeds : IDependency
    {
        bool DoConcats();

        bool Truncate();

        bool Populate();
        bool Populate2();
    }
}
