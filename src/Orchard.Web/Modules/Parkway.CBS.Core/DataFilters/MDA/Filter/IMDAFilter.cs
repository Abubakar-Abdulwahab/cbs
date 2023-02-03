using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MDAModel = Parkway.CBS.Core.Models.MDA;

namespace Parkway.CBS.Core.DataFilters.MDA.Filter
{
    public interface IMDAFilter : IDependency
    {
        string FilterName();
        Expression<Func<MDAModel, bool>> Filter();
        Expression<Func<MDAModel, bool>> Filter(string searchText);
    }
}
