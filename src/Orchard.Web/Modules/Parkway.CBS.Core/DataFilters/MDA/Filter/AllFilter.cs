using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MDAModel = Parkway.CBS.Core.Models.MDA;


namespace Parkway.CBS.Core.DataFilters.MDA.Filter
{
    public class AllFilter : IMDAFilter
    {
        private readonly string name = "All";
        private Expression<Func<MDAModel, bool>> lambda;

        public string FilterName()
        {
            return name;
        }

        public Expression<Func<MDAModel, bool>> Filter()
        {
            Expression<Func<MDAModel, bool>> lambda = r => r.Id != 0;            
            return lambda;
        }

        public Expression<Func<MDAModel, bool>> Filter(string searchText)
        {
            Expression<Func<MDAModel, bool>> lambda = r => (r.Name.Contains(searchText) || r.Code.Contains(searchText)) && r.Id != 0;
            return lambda;
        }
    }
}