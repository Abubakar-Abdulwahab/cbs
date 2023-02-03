using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MDAModel = Parkway.CBS.Core.Models.MDA;


namespace Parkway.CBS.Core.DataFilters.MDA.Order
{
    public interface IMDAOrder : IDependency
    {
        string OrderName();
        List<MDAModel> Order(List<MDAModel> mdas, bool ascending);
    }
}