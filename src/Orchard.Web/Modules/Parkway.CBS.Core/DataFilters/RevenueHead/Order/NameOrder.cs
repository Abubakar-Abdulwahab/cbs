using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;


namespace Parkway.CBS.Core.DataFilters.RevenueHead.Order
{
    public class NameOrder : IRevenueHeadOrder
    {
        private readonly string name = "Name";
        public string OrderName()
        {
            return name;
        }

        public List<RevenueHeadModel> Order(List<RevenueHeadModel> revenueHeads, bool ascending)
        {
            if (ascending) { return revenueHeads.OrderBy(keySelector => keySelector.Name).ToList(); }
            return revenueHeads.OrderByDescending(keySelector => keySelector.Name).ToList();
        }
    }
}