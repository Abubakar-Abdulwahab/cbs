using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;


namespace Parkway.CBS.Core.DataFilters.RevenueHead.Order
{
    public interface IRevenueHeadOrder : IDependency
    {
        string OrderName();
        List<RevenueHeadModel> Order(List<RevenueHeadModel> revenueHeads, bool ascending);
    }
}