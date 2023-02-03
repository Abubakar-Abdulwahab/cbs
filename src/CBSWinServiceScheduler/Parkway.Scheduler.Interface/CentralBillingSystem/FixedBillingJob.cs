using System;
using Quartz;
using NHibernate;
using System.Linq;
using System.Threading.Tasks;
using Parkway.Scheduler.Interface.Loggers.Contracts;
using Parkway.Scheduler.Interface.Schedulers.Quartz;
using System.Collections.Generic;
using Parkway.CentralBillingScheduler.DAO.Repositories.Contracts;
using Parkway.CentralBillingScheduler.DAO.Models;
using Parkway.CentralBillingScheduler.DAO.Repositories;
using Parkway.CBS.ReferenceData.Configuration;

using Parkway.Scheduler.Interface.CentralBillingSystem.HelperModels;

namespace Parkway.Scheduler.Interface.CentralBillingSystem
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    internal class FixedBillingJob : BaseJob, IJob
    {
        static int count = 0;
        public Task Execute(IJobExecutionContext context)
        {
            //if (count % 2 != 0)
            //{
            //    count++;
            //    return Task.Run(() => { });
            //}

            if (count != 30)
            {
                count++;
                return Task.Run(() => { });
            }

            count++;

            return Task.Run(() =>
            {
                FixedBillingExecution(context);
            });
        }

        /// <summary>
        /// Execution for fixed billing scheduler
        /// </summary>
        /// <param name="context"></param>
        private void FixedBillingExecution(IJobExecutionContext context)
        {
            IJobDetail jobDetail = context.JobDetail;
            ISchedulerLogger logger = SchedulerInterface.GetLoggerInstance();

            var desc = context.JobDetail.Description;
            logger.Information(string.Format("Running job SN: {3} JN: {0} JG: {1} Date: {2}", jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime(), context.Scheduler.SchedulerName));
            //we want to know if the duration termination is by endsafter rounds
            bool canStillRun = HandlerRoundsIfAny(jobDetail, context.Scheduler.SchedulerName);

            if (!canStillRun)
            {
                JobKey jobKey = new JobKey(jobDetail.Key.Name, jobDetail.Key.Group);
                //deletes job and associated triggers
                context.Scheduler.DeleteJob(jobKey);
                logger.Information(string.Format("Job has been deleted JN: {0} JG: {1}", jobKey.Name, jobKey.Group));
            }

            ISession session = GetSession(context.Scheduler.SchedulerName.Split('_')[0] + "_SessionFactory");

            try
            {
                logger.Information(string.Format("Starting reference data acquiring for SN: {0} JN: {1} JG: {2} Time: {3}.Getting job details", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy")));
                //lets get the tenant details for this job
                CBSTenantSettings tenant = new CBSTenantSettings();
                //get tenant
                IRepository<CBSTenantSettings> _tenantRepo = new Repository<CBSTenantSettings>(session);
                tenant = _tenantRepo.Get(s => s.Id != 0).FirstOrDefault();
                if (tenant == null)
                {
                    logger.Error(string.Format("Tenant info was null for SN: {0} JN: {1} JG: {2} Time: {3}", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy")));
                    return;
                }
                //get revenue head
                IRepository<RevenueHead> _revenueHeadRepo = new Repository<RevenueHead>(session);
                var revenueHeadId = 0;
                var parsed = Int32.TryParse(jobDetail.Key.Group.Split(new string[] { "RevenueHead" }, 2, StringSplitOptions.None)[1], out revenueHeadId);
                RevenueHead revenueHead = _revenueHeadRepo.Get(revenueHeadId);
                if (revenueHead == null)
                {
                    logger.Error(string.Format("Revenue head info of value {4} was null for SN: {0} JN: {1} JG: {2} Time: {3}", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy"), revenueHeadId));
                    return;
                }
                var mda = revenueHead.Mda;
                if (mda == null)
                {
                    logger.Error(string.Format("Revenue head mda info of RH value {4} was null for SN: {0} JN: {1} JG: {2} Time: {3}", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy"), revenueHeadId));
                    return;
                }
                var billing = revenueHead.BillingModel;
                if (billing == null)
                {
                    logger.Error(string.Format("Revenue head billing info of RH value {4} was null for SN: {0} JN: {1} JG: {2} Time: {3}", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy"), revenueHeadId));
                    return;
                }
                string tableName = "Parkway_CBS_Core_RefDataTemp";
                string batchNumber = GetBatchNumber(context.Scheduler.SchedulerName.Split('_')[0]);
                //we have gotten work info, let see what reference data source has been configured
                List<RefDataTemp> entities = new List<RefDataTemp>();
                //entities = GetRefData(tenant.ReferenceDataSourceName, revenueHeadId, batchNumber, billing.Amount, billing.Id);
                //logger.Information(string.Format("Job details gotten for job for SN: {0} JN: {1} JG: {2} Time: {3}. Batch number {4}, Entities size {5}. Persistence => ", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy"), batchNumber, entities.Count));

                //we have gotten the ref data entites
                //lets store them
                bool result = StoreRefData(tableName, entities, session);
                //lets get ref data entries and their corresponding tax entites of they exist
                ProcessResponseModel processResult = GetRefDataWithTheirCorrespondingCashflowCredentials(session, batchNumber, revenueHead.Id, billing.Id);
                //if process result has errors, update table and return
                if (processResult.HasErrors)
                {
                    Dictionary<string, dynamic> columnAndValue = new Dictionary<string, dynamic>
                {
                    { "Status", ((int)ScheduleInvoiceProcessingStatus.ErrorInGettingJoinerWithTaxEntity) },
                    { "StatusDetail", RefDataTemp.GetStatusDetails(ScheduleInvoiceProcessingStatus.ErrorInGettingJoinerWithTaxEntity) },
                    { "ErrorLog", processResult.ErrorMessage }
                };
                    UpdateRefDataTemp(session, tableName, columnAndValue, batchNumber, revenueHead.Id, billing.Id);
                    return;
                }
                //process a fixed bill
                IList<RefDataAndCashflowDetails> refDataTaxEntitiesJoiner = processResult.MethodReturnObject;
                //segments joiner into have cashflow customer details and not
                HasCashflowCustomerAndHasNot segmentResult = Utils.SegmentThoseThatHaveCashflowRecordsAndThoseThatHaveNone(refDataTaxEntitiesJoiner);
                //lets get the entites without cashflow details, remove duplicate
                ProcessResponseModel removeDuplicateResult = Utils.SegmentEntitiesWithoutCashflowRecordsIntoUniqueItemAndDuplicates(segmentResult.ItemsWithoutCashflowDetails);

                if (removeDuplicateResult.HasErrors)
                {
                    Dictionary<string, dynamic> columnAndValue = new Dictionary<string, dynamic>
                {
                    { "Status", ((int)ScheduleInvoiceProcessingStatus.ErrorGettingDistinctRefDataEntitesWithoutCashflowRecord).ToString() },
                    { "StatusDetail", RefDataTemp.GetStatusDetails(ScheduleInvoiceProcessingStatus.ErrorGettingDistinctRefDataEntitesWithoutCashflowRecord) },
                    { "ErrorLog", processResult.ErrorMessage }
                };
                    UpdateRefDataTemp(session, tableName, columnAndValue, batchNumber, revenueHead.Id, billing.Id);
                    return;
                }
                RefDataDistinctGroupModel distinctGroup = removeDuplicateResult.MethodReturnObject;
                var distinctRefDataEntities = distinctGroup.DistinctItems;
                var duplicatesRefDataEntities = distinctGroup.Duplicates;
                //List<RefDataTemp> entities = new List<RefDataTemp>();
                ////we get the date on the invoice
                //DateTime invoiceDate = DateTime.Now.ToLocalTime();
                ////we get the next time the trigger is to be fired, note if might return null based on what the settings are for billing duration model
                //var nextBillingDate = context.Trigger.GetNextFireTimeUtc();
                ////lets have a batch number to group this operation by
                ////lets get the data source configured for this revenue head
                //IEnumerable<IReferenceDataSource> _refDataSources = new List<IReferenceDataSource> { { new Mock() }, { new Adapter() }, { new ParkwayRefData() } };

                //foreach (var refSource in _refDataSources)
                //{
                //    //break;
                //    if (refSource.ReferenceDataSourceName() == tenant.ReferenceDataSourceName)
                //    {
                //        RefData refData = GetRefDataType(tenant.ReferenceDataSourceName);
                //        entities = refSource.GetActiveBillableTaxEntitesPerRevenueHead(revenueHead.Id, refData, new { BatchNumber = batchNumber, Amount = billing.Amount, BillingId = billing.Id });
                //        break;
                //    }
                //}

                //var notificationResult = NotifyRemoteServer(context);
                //if (notificationResult.StatusCode != System.Net.HttpStatusCode.OK)
                //{
                //    logger.Information(string.Format("Error on Remote system for SN: {0} JN: {1} JG: {2} Time: {3}", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy")));
                //    return;
                //    //do work here
                //    //if(notificationResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                //    //{
                //    //    context.Scheduler.DeleteJob(new JobKey(jobDetail.Key.Name, jobDetail.Key.Group));
                //    //}
                //    //context.Scheduler.DeleteJob(new JobKey(jobDetail.Key.Name, jobDetail.Key.Group));
                //}

                logger.Information(string.Format("Remote system has been notified for SN: {0} JN: {1} JG: {2} Time: {3}", context.Scheduler.SchedulerName, jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy")));
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message);
            }
            finally
            {
                if(session != null) { session.Dispose(); }
            }
        }        


        /// <summary>
        /// Check if the job detail has any rounds tied to it
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="schedulerName"></param>
        /// <returns>bool</returns>
        private bool HandlerRoundsIfAny(IJobDetail jobDetail, string schedulerName)
        {
            ISchedulerLogger logger = SchedulerInterface.GetLoggerInstance();
            object roundsObj;

            bool hasRoundsValue = jobDetail.JobDataMap.TryGetValue(JobDataKs.ROUNDS, out roundsObj);

            if (hasRoundsValue)
            {
                int rounds = 0;
                bool parsed = Int32.TryParse(roundsObj as string, out rounds);
                if (parsed)
                {
                    if (rounds == 0)
                    {
                        logger.Information(string.Format("Job has reached it's maximim rounds and is being deleted. Fixed billing has reached the maximum rounds count for job SN: {3} JN: {0} JG: {1} Date: {2}", jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime(), schedulerName));
                        return false;
                    }
                    logger.Information(string.Format("Changing rounds info {4} for job SN: {3} JN: {0} JG: {1} Date: {2}", jobDetail.Key.Name, jobDetail.Key.Group, DateTime.Now.ToLocalTime(), schedulerName, rounds--));
                    jobDetail.JobDataMap.Put(JobDataKs.ROUNDS, rounds.ToString());
                    return true;
                }
                else
                {
                    logger.Error(string.Format("Could not parse rounds value {0} for SN: {1} JN: {2} JG: {3}", roundsObj as string, schedulerName, jobDetail.Key.Name, jobDetail.Key.Group));
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Get session
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns>ISession</returns>
        private static ISession GetSession(string sessionFactoryName)
        {
            #region session manager
            var sessionManager = SchedulerInterface.GetSessionManagerInstance().GetSessionManager(sessionFactoryName);
            ISession session = null;
            try
            {
                session = sessionManager.NewNHibernateSession();
            }
            catch (Exception exception)
            {
                sessionManager = SchedulerInterface.GetSessionManagerInstance().ReInitializeSessionManager(sessionFactoryName);
                throw;
            }
            #endregion
            return session;
        }


        private RefData GetRefDataType(string referenceDataSourceName)
        {
            throw new NotImplementedException();
        }


        //private RemoteClientResponse NotifyRemoteServer(IJobExecutionContext context)
        //{
        //    //get schedule item for this scheduler
        //    try
        //    {
        //        SchedulerItem schedulerParameters = SchedulerInterface.GetScheduleItem(context.Scheduler.SchedulerName);
        //        //once we get the scheduler item, we have all the parameters we need for the remote request
        //        //lets get the revenue head first
        //        var revenueHeadId = context.JobDetail.Key.Group.Split(new string[] { "RevenueHead" }, 2, StringSplitOptions.None)[1];
        //        //we have gotten the revenue head id, lets notifiy the remote server that this schedule is ready to run
        //        //compute header signature
        //        string value = revenueHeadId + schedulerParameters.Name + schedulerParameters.ClientToken;
        //        string hash = GenerateSignature(value, schedulerParameters.ClientSecret);
        //        IRemoteClient remoteClient = new RemoteClient();
        //        return CallRemoteClient(schedulerParameters.Endpoint + "/fixed?revenueHeadId=" + revenueHeadId, "GET", new Dictionary<string, dynamic> { { "SIGNATURE", hash }, { "CLIENTID", schedulerParameters.ClientToken } });
        //    }
        //    catch (Exception exception)
        //    {

        //        throw;
        //    }

        //}

    }
}
