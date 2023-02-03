using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDAModel = Parkway.CBS.Core.Models.MDA;

namespace Parkway.CBS.Core.DataFilters.MDA.Order
{
    public class UpdatedAtUtcOrder : IMDAOrder
    {
        private readonly string name = "UpdatedAtUtc";

        public string OrderName()
        {
            return name;
        }

        public List<MDAModel> Order(List<MDAModel> mdas, bool ascending)
        {
            if (ascending) { return mdas.OrderBy(keySelector => keySelector.UpdatedAtUtc).ToList(); }
            return mdas.OrderByDescending(keySelector => keySelector.UpdatedAtUtc).ToList();
        }
    }
}