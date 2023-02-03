using Orchard;
using System;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;


namespace Parkway.CBS.Core.DataFilters.RevenueHead.Filter
{
    public interface IRevenueHeadFilter : IDependency
    {
        string FilterName();
        Func<RevenueHeadModel, bool> Filter();
        Func<RevenueHeadModel, bool> Filter(string searchText);
    }
}