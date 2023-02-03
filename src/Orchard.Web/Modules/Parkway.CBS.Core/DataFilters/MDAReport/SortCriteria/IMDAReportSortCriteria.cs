using Orchard;
using System;
using System.Linq.Expressions;
using MDAModel = Parkway.CBS.Core.Models.MDA;

namespace Parkway.CBS.Core.DataFilters.MDAReport.SortCriteria
{
    public interface IMDAReportSortCriteria : IDependency
    {
        string FilterName();
        Expression<Func<MDAModel, bool>> Filter();
        Expression<Func<MDAModel, bool>> Filter(string searchText);
    }
}
