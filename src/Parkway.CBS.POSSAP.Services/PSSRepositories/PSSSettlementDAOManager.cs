using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementDAOManager : Repository<PSSSettlement>, IPSSSettlementDAOManager
    {
        private static readonly ILogger log = new Log4netLogger();

        public PSSSettlementDAOManager(IUoW uow) : base(uow) { }


        //public List<PSSSettlementRuleVM> fGetBatchActivePOSSAPSettlements(int chunkSize, int skip, DateTime startDate, DateTime endDate)
        //{
        //    List<PSSSettlementRuleVM> v = new List<PSSSettlementRuleVM> { };

        //    for (int i = 0; i < chunkSize; i++)
        //    {
        //        v.Add(new PSSSettlementRuleVM { PSSSettlementId = i + chunkSize + skip, NextScheduleDate = DateTime.Now });
        //    }
        //    return v;

        //    //return _uow.Session.Query<PSSSettlement>()
        //    //    .Where(x => x.IsActive && x.SettlementRule.NextScheduleDate >= startDate && x.SettlementRule.NextScheduleDate <= endDate).Skip(skip).Take(chunkSize)
        //    //    .Select(x => new PSSSettlementRuleVM
        //    //    {
        //    //        PSSSettlementId = x.Id,
        //    //        SettlemntRuleId = x.SettlementRule.Id,
        //    //        Name = x.Name,
        //    //        SettlementEngineRuleIdentifier = x.SettlementRule.SettlementEngineRuleIdentifier,
        //    //        CronExpression = x.SettlementRule.CronExpression,
        //    //        NextScheduleDate = x.SettlementRule.NextScheduleDate
        //    //    });
        //}


        /// <summary>
        /// Get paginated records of all the active POSSAP settlement configurations on the systems.
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="date"></param>
        /// <returns>IEnumerable<PSSSettlementRuleVM></returns>
        public IEnumerable<PSSSettlementRuleVM> GetBatchActivePOSSAPSettlements(int chunkSize, int skip, DateTime startDate, DateTime endDate)
        {
            return _uow.Session.Query<PSSSettlement>()
                .Where(x => x.IsActive && x.SettlementRule.NextScheduleDate >= startDate && x.SettlementRule.NextScheduleDate <= endDate).Skip(skip).Take(chunkSize)
                .Select(x => new PSSSettlementRuleVM
                {
                    PSSSettlementId = x.Id,
                    SettlemntRuleId = x.SettlementRule.Id,
                    Name = x.Name,
                    SettlementEngineRuleIdentifier = x.SettlementRule.SettlementEngineRuleIdentifier,
                    CronExpression = x.SettlementRule.CronExpression,
                    NextScheduleDate = x.SettlementRule.NextScheduleDate,
                    SettlementPeriodStartDate = x.SettlementRule.SettlementPeriodStartDate,
                    SettlementPeriodEndDate = x.SettlementRule.SettlementPeriodEndDate,
                    HasCommandSplits = x.HasCommandSplits
                });
        }


        /// <summary>
        /// Save the settlement batch along with the hangfire reference
        /// </summary>
        /// <param name="settlementBatchKeyPairs"></param>
        /// <returns>return true if saved successfully, else return false if anything goes wrong</returns>
        public bool SaveSettlementBatchAndHangFireRef(List<KeyValuePair<PSSSettlementBatch, PSSHangfireSettlementReference>> settlementBatchKeyPairs)
        {
            try
            {
                _uow.BeginStatelessTransaction();
                foreach (var item in settlementBatchKeyPairs)
                {
                    _uow.Session.Save(item.Key);
                    item.Value.ReferenceId = item.Key.Id;
                    _uow.Session.Save(item.Value);
                }
                _uow.Commit();
                log.Info($"Saved records for settlement batch and hangfire settlement reference");
                return true;
            }
            catch (Exception exception)
            {
                _uow.Rollback();
                log.Error($"Error inserting records settlement batch and hangfire settlement reference", exception);
                return false;
            }
        }


        /// <summary>
        /// Save the collection
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>bool | return true if the collection save successfully, else false</returns>
        public bool SavePSSSettlementBatch(List<PSSSettlementBatch> batch)
        {
            try
            {
                _uow.BeginStatelessTransaction();
                foreach (var item in batch)
                {
                    _uow.Session.Save(item);
                }
                _uow.Commit();
                log.Info($"Saved records for settlement batch");
                return true;
            }
            catch (Exception exception)
            {
                _uow.Rollback();
                log.Error($"Error inserting records settlement batch", exception);
                return false;
            }
        }
    }
}
