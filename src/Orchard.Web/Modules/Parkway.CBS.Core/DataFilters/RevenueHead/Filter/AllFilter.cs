using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Orchard.Logging;
using RevenueHeadModel = Parkway.CBS.Core.Models.RevenueHead;
using System.Text.RegularExpressions;

namespace Parkway.CBS.Core.DataFilters.RevenueHead.Filter
{
    public class AllFilter : IRevenueHeadFilter
    {
        public string FilterName()
        {
            return "All";
        }

        public Func<RevenueHeadModel, bool> Filter()
        {
            Expression<Func<RevenueHeadModel, bool>> lambda = r => r.Id != 0;
            return lambda.Compile();
        }

        public Func<RevenueHeadModel, bool> Filter(string searchText)
        {
            Expression<Func<RevenueHeadModel, bool>> lambda = r => (Regex.IsMatch(r.Name,".*"+searchText+".*", RegexOptions.IgnoreCase) || Regex.IsMatch(r.Code, ".*" + searchText + ".*", RegexOptions.IgnoreCase)) && r.Id != 0;
            return lambda.Compile();
        }

        //public Func<DAORevenueHead, object> Order(string orderBy)
        //{
        //    try
        //    {
        //        ParameterExpression argParam = Expression.Parameter(typeof(DAORevenueHead), "keySelector");
        //        Expression orderProp = Expression.Property(argParam, orderBy);

        //        var orderLambda = Expression.Lambda<Func<DAORevenueHead, object>>(orderProp, argParam);
        //        return orderLambda.Compile();
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Error(exception, Lang.applicationexception_text.ToString());
        //        return null;
        //    }
            
        //}
    }
}