using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface IFormControlsSeeds : IDependency
    {
        /// <summary>
        /// Adds records to the forms control table for update 2
        /// <para>Returns int 3</para>
        /// </summary>
        void Seed1();
    }
}
