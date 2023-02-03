using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDAModel = Parkway.CBS.Core.Models.MDA;


namespace Parkway.CBS.Core.DataFilters.MDA.Order
{
    public class CodeOrder : IMDAOrder
    {
        private readonly string name = "Code";

        public string OrderName()
        {
            return name;
        }

        public List<MDAModel> Order(List<MDAModel> mdas, bool ascending)
        {
            if (ascending) { return mdas.OrderBy(keySelector => keySelector.Code).ToList(); }
            return mdas.OrderByDescending(keySelector => keySelector.Code).ToList();
        }
    }
}