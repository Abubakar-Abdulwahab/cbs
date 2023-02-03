using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface ITINSeeds : IDependency
    {
        bool Seed1();
        bool Seed2();

        bool Seed3();

        string Seed4();

    }
}
