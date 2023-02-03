using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface ILGASeeds : IDependency
    {
        void PopLGAs();

        void AddLgas(List<Dictionary<string, string>> lgaList);

 
    }
}
