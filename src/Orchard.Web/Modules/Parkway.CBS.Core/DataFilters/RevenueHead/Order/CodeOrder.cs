using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;


namespace Parkway.CBS.Core.DataFilters.RevenueHead.Order
{
    public class CodeOrder : IRevenueHeadOrder
    {
        private readonly string name = "Code";

        public string OrderName()
        {
            return name;
        }

        public List<RevenueHeadModel> Order(List<RevenueHeadModel> revenueHeads, bool ascending)
        {
            if (ascending) { return revenueHeads.OrderBy(keySelector => keySelector.Code).ToList(); }
            return revenueHeads.OrderByDescending(keySelector => keySelector.Code).ToList();
        }
    }
}