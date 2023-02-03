using System;
using System.Linq.Expressions;
using MDAModel = Parkway.CBS.Core.Models.MDA;


namespace Parkway.CBS.Core.DataFilters.MDA.Filter
{
    public class EnabledFilter : IMDAFilter
    {
        public readonly string name = "Enabled";

        public string FilterName()
        {
            return name;
        }

        public Expression<Func<MDAModel, bool>> Filter()
        {
            Expression<Func<MDAModel, bool>> lambda = r => r.IsActive == true;
            return lambda;
        }

        public Expression<Func<MDAModel, bool>> Filter(string searchText)
        {
            Expression<Func<MDAModel, bool>> lambda = r => (r.Name.Contains(searchText) || r.Code.Contains(searchText)) && r.IsActive == true;
            return lambda;
        }
    }
}