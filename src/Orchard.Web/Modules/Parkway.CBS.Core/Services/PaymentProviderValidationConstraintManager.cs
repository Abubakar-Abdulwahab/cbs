using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class PaymentProviderValidationConstraintManager : BaseManager<PaymentProviderValidationConstraint>, IPaymentProviderValidationConstraintManager<PaymentProviderValidationConstraint>
    {

        private readonly IRepository<PaymentProviderValidationConstraint> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PaymentProviderValidationConstraintManager(IRepository<PaymentProviderValidationConstraint> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// This method queries the database to check if the the combination of the
        /// mda, revenue head and payment provider has any stored record. It returns the count.
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="paymentProviderId"></param>
        /// <returns>int</returns>
        public int CountNumberOfValidationRestrictions(int mdaId, int revenueHeadId, int paymentProviderId)
        {
            return _transactionManager.GetSession()
                .Query<PaymentProviderValidationConstraint>()
                .Where(val => ((val.MDA == new MDA { Id = mdaId }) && (val.PaymentProvider == new ExternalPaymentProvider { Id = paymentProviderId }) &&
                ((val.RevenueHead == new RevenueHead { Id = revenueHeadId }) || (val.RevenueHead == null))))
                .Count();
        }

        /// <summary>
        /// This method fetches existing validation constraints for the payment provider with the specified Id.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public IEnumerable<PaymentProviderValidationConstraintsVM> GetExistingConstraints(int providerId)
        {
            return _transactionManager.GetSession().Query<PaymentProviderValidationConstraint>().Where(ppvc => ppvc.PaymentProvider.Id == providerId && ppvc.IsDeleted == false)
                .Select(ppvc => new PaymentProviderValidationConstraintsVM { MDAId = ppvc.MDA.Id, RevenueHeadId = (ppvc.RevenueHead == null) ? 0 : ppvc.RevenueHead.Id });
        }


        /// <summary>
        /// Update payment provider constraints records with changes on the MDARevenueAccessRestrictionsStaging table for the payment provider with specified Id using the specified MDARevenueHeadEntryStagingId.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="MDARevenueHeadEntryStagingId"></param>
        public void UpdateProviderRecords(int providerId, Int64 MDARevenueHeadEntryStagingId)
        {
            string targetTable = "Parkway_CBS_Core_" + typeof(PaymentProviderValidationConstraint).Name;
            string sourceTable = "Parkway_CBS_Core_" + typeof(MDARevenueAccessRestrictionsStaging).Name;

            var queryText = $"MERGE {targetTable} AS TARGET USING {sourceTable} AS SOURCE ON((TARGET.RevenueHead_Id = SOURCE.RevenueHead_Id OR(TARGET.RevenueHead_Id IS NULL AND SOURCE.RevenueHead_Id IS NULL)) AND TARGET.PaymentProvider_Id = :providerId AND TARGET.MDA_Id = SOURCE.MDA_Id) WHEN MATCHED AND SOURCE.MDARevenueHeadEntryStaging_Id = :MDARevenueHeadEntryStagingId THEN UPDATE SET TARGET.IsDeleted = SOURCE.IsRemoval, TARGET.UpdatedAtUtc = :updateDate WHEN NOT MATCHED BY TARGET AND SOURCE.MDARevenueHeadEntryStaging_Id = :MDARevenueHeadEntryStagingId THEN INSERT(MDA_Id, RevenueHead_Id, LastUpdatedBy_Id, CreatedAtUtc, UpdatedAtUtc, PaymentProvider_Id, IsDeleted) VALUES(SOURCE.MDA_Id, SOURCE.RevenueHead_Id, SOURCE.LastUpdatedBy_Id, :updateDate, :updateDate, :providerId, SOURCE.IsRemoval);";

            var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
            query.SetParameter("providerId", providerId);
            query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
            query.SetParameter("MDARevenueHeadEntryStagingId", MDARevenueHeadEntryStagingId);

            query.ExecuteUpdate();
        }

    }
}