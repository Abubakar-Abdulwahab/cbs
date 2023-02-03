﻿using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITINManager<TIN> : IDependency, IBaseManager<TIN>
    {
        
    }
}
