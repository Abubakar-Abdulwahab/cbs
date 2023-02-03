using System;
using System.Linq.Expressions;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;
using System.Text.RegularExpressions;

namespace Parkway.CBS.Core.DataFilters.RevenueHead.Filter
{
    public class EnabledFilter : IRevenueHeadFilter
    {
        public string FilterName()
        {
            return "Enabled";
        }

        public Func<RevenueHeadModel, bool> Filter()
        {
            Expression<Func<RevenueHeadModel, bool>> lambda = r => r.IsActive == true;
            return lambda.Compile();
        }

        public Func<RevenueHeadModel, bool> Filter(string searchText)
        {
            Expression<Func<RevenueHeadModel, bool>> lambda = r => (Regex.IsMatch(r.Name, ".*" + searchText + ".*", RegexOptions.IgnoreCase) || Regex.IsMatch(r.Code, ".*" + searchText + ".*", RegexOptions.IgnoreCase)) && r.IsActive == true;
            return lambda.Compile();
        }
    }
}