using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;
using System.Text.RegularExpressions;

namespace Parkway.CBS.Core.DataFilters.RevenueHead.Filter
{
    public class DisabledFilter : IRevenueHeadFilter
    {
        public string FilterName()
        {
            return "Disabled";
        }

        public Func<RevenueHeadModel, bool> Filter()
        {
            Expression<Func<RevenueHeadModel, bool>> lambda = r => r.IsActive == false;
            return lambda.Compile();
        }

        public Func<RevenueHeadModel, bool> Filter(string searchText)
        {
            Expression<Func<RevenueHeadModel, bool>> lambda = r => (Regex.IsMatch(r.Name, ".*" + searchText + ".*", RegexOptions.IgnoreCase) || Regex.IsMatch(r.Code, ".*" + searchText + ".*", RegexOptions.IgnoreCase)) && r.IsActive == false;
            return lambda.Compile();
        }
    }
}