using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport.SearchFilters
{
    public class PaymentReferecneReportFilter : IDeploymentAllowancePaymentReportFilters
    {
        /// <summary>
        /// Applies the payment reference search filter and gets only items with matching payment reference
        /// </summary>
        public void AddCriteriaRestriction(ICriteria criteria, DeploymentAllowancePaymentSearchParams searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.PaymentRef))
            {
                criteria.Add(Restrictions.Eq($"{nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest.PaymentReference)}", searchParam.PaymentRef));
            }
        }
    }

    public class SourceAccountNameReportFilter : IDeploymentAllowancePaymentReportFilters
    {
        /// <summary>
        /// Applies the source account name search filter and gets only items with matching source account names
        /// </summary>
        public void AddCriteriaRestriction(ICriteria criteria, DeploymentAllowancePaymentSearchParams searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.SourceAccountName))
            {
                criteria.Add(Restrictions.InsensitiveLike($"{nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest.AccountName)}", $"%{searchParam.SourceAccountName}%", MatchMode.Anywhere));
            }
        }
    }

    public class PaymentStatusReportFilter : IDeploymentAllowancePaymentReportFilters
    {
        /// <summary>
        /// Applies the payment status search filter and gets only items with matching payment status
        /// </summary>
        public void AddCriteriaRestriction(ICriteria criteria, DeploymentAllowancePaymentSearchParams searchParam)
        {
            if (searchParam.Status > 0)
            {
                criteria.Add(Restrictions.Where<DeploymentAllowancePaymentRequestItem>(x => x.TransactionStatus == searchParam.Status));
            }
        }
    }

    public class CommandTypeReportFilter : IDeploymentAllowancePaymentReportFilters
    {
        /// <summary>
        /// Applies the commandType search filter and gets only items with matching command types
        /// </summary>
        public void AddCriteriaRestriction(ICriteria criteria, DeploymentAllowancePaymentSearchParams searchParam)
        {
            if (searchParam.CommandTypeId > 0)
            {
                criteria.Add(Restrictions.Where<DeploymentAllowancePaymentRequestItem>(x => x.CommandType.Id == searchParam.CommandTypeId));
            }
        }
    }
}