using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDAModel = Parkway.CBS.Core.Models.MDA;

namespace Parkway.CBS.Core.DataFilters.MDAReport.Order
{
    public interface IMDAReportOrder : IDependency
    {
        string OrderName();
        List<MDAModel> Order(List<MDAModel> mdas, bool ascending);
    }
}