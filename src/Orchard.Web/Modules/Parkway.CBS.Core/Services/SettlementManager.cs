using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace Parkway.CBS.Core.Services
{
    public class SettlementRuleManager : BaseManager<SettlementRule>, ISettlementRuleManager<SettlementRule>
    {

        private readonly IRepository<SettlementRule> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public SettlementRuleManager(IRepository<SettlementRule> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Get the settlements that have the given parameters
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="paymentProvider_Id"></param>
        /// <param name="paymentChannel_Id"></param>
        /// <returns>IEnumerable{SettlementRuleVM}</returns>
        public IEnumerable<SettlementRuleVM> GetParentSettlements(MDA mda, RevenueHead revenueHead, int paymentProvider_Id, int paymentChannel_Id)
        {
            throw new NotImplementedException();
            //return _transactionManager.GetSession()
            //    .Query<SettlementRule>().Where(r => ((r.MDA == mda) &&(r.RevenueHead == revenueHead) && (r.PaymentProvider_Id == paymentProvider_Id) && (r.PaymentChannel_Id == paymentChannel_Id)))
            //    .Select(r => new SettlementRuleVM
            //    {
            //         Id = r.Id,
            //         Weight = r.SettlementHierarchyLevel
            //    }).ToFuture();
        }


        public bool SaveRoot(SettlementRule model)
        {
            //try
            //{
            //    string queryText =
            //        "INSERT INTO [dbo].[Parkway_CBS_Core_SettlementRule] ([Name],[SettlementEngineRuleIdentifier],[RevenueHead_Id],[MDA_Id],[PaymentProvider],[PaymentChannel],[AddedBy_Id],[ConfirmedBy_Id],[CronExpression],[NextScheduleDate],[CompositeUnique],[JSONScheduleModel],[IsActive],[CreatedAtUtc],[UpdatedAtUtc],[SettlementHierarchyNode]) " +
            //        "VALUES (:name, :identifier, :revId, :mdaId, :paymentProvider, :paymentChannel, :addedBy, :confirmedBy, :cronExpression, :nextScheduleDate, :compositeUnique, :jsonModel, :isActive, :date , :date, CAST('/' AS hierarchyid))";

            //    var insertQuery = _transactionManager.GetSession().CreateSQLQuery(queryText);
            //    insertQuery.SetParameter("name", model.Name);
            //    insertQuery.SetParameter("identifier", model.SettlementEngineRuleIdentifier);
            //    insertQuery.SetParameter("revId", model.RevenueHead?.Id);
            //    insertQuery.SetParameter("mdaId", model.MDA?.Id);
            //    insertQuery.SetParameter("paymentProvider", model.PaymentProvider_Id);
            //    insertQuery.SetParameter("paymentChannel", model.PaymentChannel_Id);
            //    insertQuery.SetParameter("addedBy", model.AddedBy.Id);
            //    insertQuery.SetParameter("confirmedBy", model.AddedBy.Id);
            //    insertQuery.SetParameter("cronExpression", model.CronExpression);
            //    insertQuery.SetParameter("nextScheduleDate", model.NextScheduleDate);
            //    insertQuery.SetParameter("compositeUnique", model.CompositeUnique);
            //    insertQuery.SetParameter("jsonModel", model.JSONScheduleModel);
            //    insertQuery.SetParameter("isActive", model.IsActive);
            //    insertQuery.SetParameter("date", model.CreatedAtUtc);

            //    insertQuery.ExecuteUpdate();
            //}
            //catch (Exception exception)
            //{
            //    _transactionManager.GetSession().Transaction.Rollback();
            //    Logger.Error(exception, exception.Message);
            //    return false;
            //}
            //return true;
            throw new NotImplementedException();
        }


        
        //public override bool Save(Models.SettlementRule persistModel)
        //{
        //    throw new Exception { };
        //}

    }
}