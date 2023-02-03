using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using MDAModel = Parkway.CBS.Core.Models.MDA;


namespace Parkway.CBS.Core.DataFilters.MDA.Filter
{
    public class VisibleFilter : IMDAFilter
    {
        public readonly string name = "Visible";

        public string FilterName()
        {
            return name;
        }

        public Expression<Func<MDAModel, bool>> Filter()
        {
            Expression<Func<MDAModel, bool>> lambda = r => ((r.IsActive == true) && (r.IsVisible == true));
            return lambda;
        }

        public Expression<Func<MDAModel, bool>> Filter(string searchText)
        {
            Expression<Func<MDAModel, bool>> lambda = r => (r.Name.Contains(searchText) || r.Code.Contains(searchText)) && ((r.IsActive == true) 
                                                        && (r.IsVisible == true));
            return lambda;
        }
    }
}