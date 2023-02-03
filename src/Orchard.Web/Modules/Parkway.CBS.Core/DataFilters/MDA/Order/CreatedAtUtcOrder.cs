using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDAModel = Parkway.CBS.Core.Models.MDA;


namespace Parkway.CBS.Core.DataFilters.MDA.Order
{
    public class CreatedAtUtcOrder : IMDAOrder
    {
        private readonly string name = "CreatedAtUtc";

        public string OrderName()
        {
            return name;
        }

        public List<MDAModel> Order(List<MDAModel> mdas, bool ascending)
        {
            if (ascending) { return mdas.OrderBy(keySelector => keySelector.CreatedAtUtc).ToList(); }
            return mdas.OrderByDescending(keySelector => keySelector.CreatedAtUtc).ToList();
        }

        
    }
}