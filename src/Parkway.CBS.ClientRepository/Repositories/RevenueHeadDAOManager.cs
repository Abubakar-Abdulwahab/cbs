using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class RevenueHeadDAOManager : Repository<RevenueHead>, IRevenueHeadDAOManager
    {
        public RevenueHeadDAOManager(IUoW uow) : base(uow)
        { }

        public RevenueHeadDetailsForInvoiceGenerationLite GetRevenueHeadDetailsForInvoiceGeneration(long revenueHeadId)
        {
            return _uow.Session.Query<RevenueHead>()
                .Where(revh => (revh.Id == revenueHeadId && (revh.IsActive)))
                .Select(rev => new RevenueHeadDetailsForInvoiceGenerationLite
                {
                    CashFlowProductId = rev.CashFlowProductId,
                    JSONBillingDiscounts = rev.BillingModel.Discounts,
                    SMEKey = rev.Mda.SMEKey,
                    JSONBillingPenalties = rev.BillingModel.Penalties,
                    RevenueHeadNameAndCode = rev.NameAndCode(),
                    JSONDueDate = rev.BillingModel.DueDate,
                    NextBillingDate = rev.BillingModel.NextBillingDate,
                    BillingModelId = rev.BillingModel.Id,
                    RevenueHeadId = rev.Id,
                    MDAId = rev.Mda.Id,
                    ExpertSystemId = rev.Mda.ExpertSystemSettings.Id,
                })
                .ToList().Single();
        }


        /// <summary>
        /// Get the model for invoice generation for payee assessment
        /// </summary>
        /// <returns>InvoicingServiceInvoiceGenerationModel</returns>
        public RevenueHeadDetailsForInvoiceGenerationLite GetRevenueHeadDetailsForInvoiceGenerationForPayee()
        {
            return _uow.Session.Query<RevenueHead>()
                .Where(revh => ((revh.IsPayeAssessment) && (revh.IsActive) && (revh.IsVisible)))
                .Select(rev => new RevenueHeadDetailsForInvoiceGenerationLite
                {
                    CashFlowProductId = rev.CashFlowProductId,
                    JSONBillingDiscounts = rev.BillingModel.Discounts,
                    SMEKey = rev.Mda.SMEKey,
                    JSONBillingPenalties = rev.BillingModel.Penalties,
                    RevenueHeadNameAndCode = rev.NameAndCode(),
                    JSONDueDate = rev.BillingModel.DueDate,
                    NextBillingDate = rev.BillingModel.NextBillingDate,
                    BillingModelId = rev.BillingModel.Id,
                    RevenueHeadId = rev.Id,
                    MDAId = rev.Mda.Id,
                    ExpertSystemId = rev.Mda.ExpertSystemSettings.Id,
                })
                .ToList().Single();
        }

        public RevenueHeadDetailsForInvoiceGenerationLite GetRevenueHeadDetailsForInvoiceGenerationForPayee(long revenueHeadId)
        {
            return _uow.Session.Query<RevenueHead>()
                .Where(revh => (revh.Id == revenueHeadId && (revh.IsActive) && (revh.IsVisible)))
                .Select(rev => new RevenueHeadDetailsForInvoiceGenerationLite
                {
                    CashFlowProductId = rev.CashFlowProductId,
                    JSONBillingDiscounts = rev.BillingModel.Discounts,
                    SMEKey = rev.Mda.SMEKey,
                    JSONBillingPenalties = rev.BillingModel.Penalties,
                    RevenueHeadNameAndCode = rev.NameAndCode(),
                    JSONDueDate = rev.BillingModel.DueDate,
                    NextBillingDate = rev.BillingModel.NextBillingDate,
                    BillingModelId = rev.BillingModel.Id,
                    RevenueHeadId = rev.Id,
                    MDAId = rev.Mda.Id,
                    ExpertSystemId = rev.Mda.ExpertSystemSettings.Id,
                })
                .ToList().Single();
        }


        /// <summary>
        /// Get the group revenue head details
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>RevenueHeadForInvoiceGenerationHelper</returns>
        public GenerateInvoiceRequestModel GetGroupRevenueHeadVMForInvoiceGeneration(int groupId)
        {
            var result = _uow.Session.Query<RevenueHead>().Where(revh => (revh.Id == groupId) && (revh.IsGroup)).Select(rev => new GenerateInvoiceRequestModel()
            {
                BillingModelVM = new BillingModelVM
                { BillingType = rev.BillingModel.GetBillingType(), DueDate = rev.BillingModel.DueDate, StillRunning = rev.BillingModel.StillRunning, Id = rev.BillingModel.Id, PenaltyJSONModel = rev.BillingModel.Penalties, DiscountJSONModel = rev.BillingModel.Discounts, NextBillingDate = rev.BillingModel.NextBillingDate },
                RevenueHeadVM = new RevenueHeadVM { Code = rev.Code, Id = rev.Id, Name = rev.Name, InvoiceGenerationRedirectURL = rev.InvoiceGenerationRedirectURL, CashflowProductId = rev.CashFlowProductId },
                MDAVM = new MDAVM { Name = rev.Mda.Name, Id = rev.Mda.Id, Code = rev.Mda.Code, SMEKey = rev.Mda.SMEKey },
                RevenueHeadGroupVM = rev.GroupParent.Select(gp => new RevenueHeadGroupVM { RevenueHeadsInGroup = gp.RevenueHead.Id }).ToList()
            }).ToList();

            if (result.Count() == 1)
            { return result.Single(); }
            return null;
        }


        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, and billing info</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        public IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadVMForInvoiceGeneration(int id)
        {
            return _uow.Session.Query<RevenueHead>().Where(revh => (revh.Id == id)).Select(rev => new RevenueHeadForInvoiceGenerationHelper()
            {
                BillingModelVM = new BillingModelVM
                { Amount = rev.BillingModel.Amount, BillingType = rev.BillingModel.GetBillingType(), DueDate = rev.BillingModel.DueDate, StillRunning = rev.BillingModel.StillRunning, Id = rev.BillingModel.Id, PenaltyJSONModel = rev.BillingModel.Penalties, DiscountJSONModel = rev.BillingModel.Discounts, NextBillingDate = rev.BillingModel.NextBillingDate },
                RevenueHeadVM = new RevenueHeadVM
                { Code = rev.Code, Id = rev.Id, Name = rev.Name, InvoiceGenerationRedirectURL = rev.InvoiceGenerationRedirectURL, CashflowProductId = rev.CashFlowProductId },
                MDAVM = new MDAVM
                { Id = rev.Mda.Id, SMEKey = rev.Mda.SMEKey }
            }
            ).ToFuture();
        }
    }
}
