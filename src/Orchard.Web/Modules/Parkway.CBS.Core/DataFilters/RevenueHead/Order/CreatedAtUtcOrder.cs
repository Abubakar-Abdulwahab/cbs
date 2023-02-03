using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;


namespace Parkway.CBS.Core.DataFilters.RevenueHead.Order
{
    public class CreatedAtUtcOrder : IRevenueHeadOrder
    {
        private readonly string name = "CreatedAtUtc";
        public string OrderName()
        {
            return name;
        }

        public List<RevenueHeadModel> Order(List<RevenueHeadModel> revenueHeads, bool ascending)
        {
            if (ascending) { return revenueHeads.OrderBy(keySelector => keySelector.CreatedAtUtc).ToList(); }
            return revenueHeads.OrderByDescending(keySelector => keySelector.CreatedAtUtc).ToList();
        }

        
    }
}