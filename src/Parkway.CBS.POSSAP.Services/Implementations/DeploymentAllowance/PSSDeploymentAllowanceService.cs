using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement
{
    public class PSSDeploymentAllowanceService
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPoliceofficerDeploymentAllowanceTrackerDAOManager deploymentAllowanceTrackerDAOManager { get; set; }

        public IPoliceOfficerDeploymentLogDAOManager policeOfficerDeploymentLogDAOManager { get; set; }

        public IDeploymentAllowanceDAOManager deploymentAllowanceDAOManager { get; set; }


        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSDeploymentAllowanceService");
            }
        }

        private void SetDeploymentAllowanceTrackerDAOManager()
        {
            if (deploymentAllowanceTrackerDAOManager == null) { deploymentAllowanceTrackerDAOManager = new PoliceofficerDeploymentAllowanceTrackerDAOManager(UoW); }
        }

        private void SetPoliceOfficerDeploymentLogDAOManager()
        {
            if (policeOfficerDeploymentLogDAOManager == null) { policeOfficerDeploymentLogDAOManager = new PoliceOfficerDeploymentLogDAOManager(UoW); }
        }

        private void SetDeploymentAllowanceDAOManager()
        {
            if (deploymentAllowanceDAOManager == null) { deploymentAllowanceDAOManager = new DeploymentAllowanceDAOManager(UoW); }
        }

        /// <summary>
        /// Start deployment allowance processing
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public string BeginDeploymentAllowanceProcessing(string tenantName)
        {
            log.Info($"About to start POSSAP deployment allowance processing");
            try
            {
                SetUnitofWork(tenantName);
                SetDeploymentAllowanceTrackerDAOManager();

                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                Int64 recordCount = deploymentAllowanceTrackerDAOManager.Count(x => !x.IsSettlementCompleted && x.NextSettlementDate.Date == today);
                log.Info($"{recordCount} deployment allowance due for settlement for {today:dd/MM/yyyy}");
                if (recordCount < 1) return $"No deployment allowance due for {today:dd/MM/yyyy}";

                SetPoliceOfficerDeploymentLogDAOManager();
                SetDeploymentAllowanceDAOManager();
                var chunkSizeStringValue = ConfigurationManager.AppSettings["ChunkSize"];
                int chunkSize = 100;

                if (!string.IsNullOrEmpty(chunkSizeStringValue))
                {
                    int.TryParse(chunkSizeStringValue, out chunkSize);
                }

                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;
                StartHangfireServer();
                UoW.BeginTransaction();
                List<PSSDeploymentAllowanceTrackerVM> deploymentTrackers = null;
                ConcurrentQueue<PoliceOfficerDeploymentAllowanceVM> deploymentAllowances = new ConcurrentQueue<PoliceOfficerDeploymentAllowanceVM>();
                while (stopper < pages)
                {
                    deploymentTrackers = deploymentAllowanceTrackerDAOManager.GetBatchDueDeploymentAllowance(chunkSize, skip, today);
                    foreach (PSSDeploymentAllowanceTrackerVM deploymentTracker in deploymentTrackers)
                    {
                        log.Info($"About to process deployment allowance. Request Id: {deploymentTracker.RequestId}");
                        List<PoliceOfficerDeploymentVM> officerDeploymentVMs = policeOfficerDeploymentLogDAOManager.GetDeployedOfficerForRequest(deploymentTracker.RequestId);
                        log.Info($"About to process deployment allowance for {officerDeploymentVMs.Count} officers for request id {deploymentTracker.RequestId}");
                        deploymentAllowances.Enqueue(ProcessDeploymentAllowance(officerDeploymentVMs, deploymentTracker));
                    }
                    deploymentTrackers.Clear();
                    skip += chunkSize;
                    stopper++;
                }

               int res = deploymentAllowanceDAOManager.SaveRecords(deploymentAllowances, chunkSize);
                UoW.Commit();
                log.Info("POSSAP deployment processing done!!!");
                return "POSSAP deployment processing done!!!";
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP deployment allowance");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
        }

        private PoliceOfficerDeploymentAllowanceVM ProcessDeploymentAllowance(List<PoliceOfficerDeploymentVM> officers, PSSDeploymentAllowanceTrackerVM allowanceTrackerVM)
        {
            try
            {
                //Check if it's only mobilization fee that has been settled i.e number of settlement done is 1, if yes, just settle the mobilization fee balance
                if (allowanceTrackerVM.NumberOfSettlementDone == (int)PSSAllowancePaymentStage.MobilizationFee)
                {
                    return ProcessDeploymentAllowanceMobilizationFeeBalance(officers, allowanceTrackerVM);
                }
                return ProcessDeploymentAllowanceMonthlyFee(officers, allowanceTrackerVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private PoliceOfficerDeploymentAllowanceVM ProcessDeploymentAllowanceMobilizationFeeBalance(List<PoliceOfficerDeploymentVM> officers, PSSDeploymentAllowanceTrackerVM allowanceTrackerVM)
        {
            try
            {
                log.Info($"About to start POSSAP mobilization balance fee deployment allowance processing. Request Id: {allowanceTrackerVM.RequestId}");

                var officerAllowanceMobilizationBalanceFee = ConfigurationManager.AppSettings["PSSOfficerAllowanceMobilizationBalanceFee"];
                var officerOperativeDirectCostPercentage = ConfigurationManager.AppSettings["PSSOfficerAllowanceDeduction"];

                string[] requiredParameters = { officerAllowanceMobilizationBalanceFee, officerOperativeDirectCostPercentage };

                if (requiredParameters.Any(x => string.IsNullOrEmpty(x)))
                {
                    log.Error($"Allowance payment configuration was not found. Request Id: {allowanceTrackerVM.RequestId}");
                    throw new Exception("Allowance payment configuration was not found");
                }

                bool parsed = decimal.TryParse(officerOperativeDirectCostPercentage, out decimal convertedOfficerOperativeDirectCostPercentage);
                if (!parsed)
                {
                    log.Error($"Unable to convert configured operative direct cost deduction percentage. Request Id: {allowanceTrackerVM.RequestId}");
                    throw new Exception("Unable to convert configured operative direct cost deduction percentage.");
                }

                parsed = decimal.TryParse(officerAllowanceMobilizationBalanceFee, out decimal convertedMobilizationBalanceFeePercentage);
                if (!parsed)
                {
                    log.Error($"Unable to convert configured allowance mobilization fee balance payment percentage. Request Id: {allowanceTrackerVM.RequestId}");
                    throw new Exception("Unable to convert configured allowance mobilization fee balance payment percentage.");
                }

                PoliceOfficerDeploymentAllowanceVM deploymentAllowance = new PoliceOfficerDeploymentAllowanceVM();
                List<DeploymentAllowanceItemVM> deploymentAllowances = new List<DeploymentAllowanceItemVM> { };
                //SettlementCycleEndDate substracted by SettlementCycleStartDate plus a day because start and end date are inclusive
                //e.g 31-01-2022 - 01-01-2022 will give 30 days without the addition of one day
                int numberOfDays = (allowanceTrackerVM.SettlementCycleEndDate - allowanceTrackerVM.SettlementCycleStartDate).Days + 1;

                foreach (PoliceOfficerDeploymentVM officer in officers)
                {
                    decimal invoiceContributedAmount = Math.Round(officer.DeploymentRate * numberOfDays, 2, MidpointRounding.ToEven);

                    string narration = $"{convertedMobilizationBalanceFeePercentage}% mobilization fee balance for {officer.OfficerName} for duration {allowanceTrackerVM.SettlementCycleStartDate.ToString("dd/MM/yyyy")} to {allowanceTrackerVM.SettlementCycleEndDate.ToString("dd/MM/yyyy")}";

                    log.Info($"Deployment allowance mobilization fee balance processing ::: {narration}. Number of days::{numberOfDays}. Request Id: {allowanceTrackerVM.RequestId}");

                    DeploymentAllowanceItemVM deploymentLog = new DeploymentAllowanceItemVM
                    {
                        Status = (int)DeploymentAllowanceStatus.PendingApproval,
                        PoliceOfficerLogId = officer.PoliceOfficerLogId,
                        Amount = ComputeAllowanceFee(invoiceContributedAmount, convertedOfficerOperativeDirectCostPercentage, convertedMobilizationBalanceFeePercentage),
                        ContributedAmount = invoiceContributedAmount,
                        Narration = narration,
                        RequestId = allowanceTrackerVM.RequestId,
                        InvoiceId = allowanceTrackerVM.InvoiceId,
                        PaymentStageId = (int)PSSAllowancePaymentStage.MobilizationBalanceFee,
                        CommandId = officer.CommandId,
                        EscortDetailsId = allowanceTrackerVM.EscortDetailId
                    };
                    deploymentAllowances.Add(deploymentLog);
                }
                deploymentAllowance.AllowanceItems = deploymentAllowances;
                deploymentAllowance.DeploymentAllowanceTrackerId = allowanceTrackerVM.Id;

                if (allowanceTrackerVM.EscortEndDate == allowanceTrackerVM.NextSettlementDate)
                {
                    //If the request is exactly one month
                    //NextSettlementDate, NextSettlementCycleStartDate and NextSettlementCycleEndDate will remain the same
                    deploymentAllowance.IsSettlementCompleted = true;
                    deploymentAllowance.NumberOfSettlementDone = allowanceTrackerVM.NumberOfSettlementDone + 1;
                    deploymentAllowance.NextSettlementDate = allowanceTrackerVM.NextSettlementDate;
                    deploymentAllowance.NextSettlementCycleStartDate = allowanceTrackerVM.SettlementCycleStartDate;
                    deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.SettlementCycleEndDate;
                }
                else if (allowanceTrackerVM.EscortEndDate <= allowanceTrackerVM.NextSettlementDate.AddMonths(1))
                {
                    //If the remaining duration is less than or equal to a month by comparing if request end date is less than or equal current NextSettlementDate plus a month
                    //Set SettlementCycleStartDate to current NextSettlementDate plus a day, then SettlementCycleEndDate to request end date, and NextSettlementDate to the request end date
                    deploymentAllowance.NumberOfSettlementDone = allowanceTrackerVM.NumberOfSettlementDone + 1;
                    deploymentAllowance.NextSettlementDate = allowanceTrackerVM.EscortEndDate;
                    deploymentAllowance.NextSettlementCycleStartDate = allowanceTrackerVM.NextSettlementDate.AddDays(1);
                    deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.EscortEndDate;
                }
                else if (allowanceTrackerVM.EscortEndDate > allowanceTrackerVM.NextSettlementDate.AddMonths(1))
                {
                    //If the remaining duration is more than a month by comparing if request end date is more than current NextSettlementDate plus a month
                    //Set SettlementCycleStartDate to current NextSettlementDate plus a day, then SettlementCycleEndDate to current NextSettlementDate plus a month, and NextSettlementDate to current NextSettlementDate plus a month
                    deploymentAllowance.NumberOfSettlementDone = allowanceTrackerVM.NumberOfSettlementDone + 1;
                    deploymentAllowance.NextSettlementCycleStartDate = allowanceTrackerVM.NextSettlementDate.AddDays(1);

                    //Check if the current NextSettlementDate is the last day of the month
                    //If yes, NextSettlementDate and NextSettlementCycleEndDate need to be end on the following month
                    //E.g if current settlement date is 30/04/2022 by adding a month it will give 30/05/2022, but we need 31/05/2022
                    int numberOfDaysInMonth = DateTime.DaysInMonth(allowanceTrackerVM.NextSettlementDate.Year, allowanceTrackerVM.NextSettlementDate.Month);
                    if(numberOfDaysInMonth == allowanceTrackerVM.NextSettlementDate.Day)
                    {
                        int nextMonthNumberOfDays = DateTime.DaysInMonth(allowanceTrackerVM.NextSettlementDate.AddDays(1).Year, allowanceTrackerVM.NextSettlementDate.AddDays(1).Month);
                        deploymentAllowance.NextSettlementDate = allowanceTrackerVM.NextSettlementDate.AddDays(nextMonthNumberOfDays);
                        deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.NextSettlementDate.AddDays(nextMonthNumberOfDays);
                    }
                    else
                    {
                        deploymentAllowance.NextSettlementDate = allowanceTrackerVM.NextSettlementDate.AddMonths(1);
                        deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.NextSettlementDate.AddMonths(1);
                    }
                }
                return deploymentAllowance;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private PoliceOfficerDeploymentAllowanceVM ProcessDeploymentAllowanceMonthlyFee(List<PoliceOfficerDeploymentVM> officers, PSSDeploymentAllowanceTrackerVM allowanceTrackerVM)
        {
            try
            {
                log.Info($"About to start POSSAP monthly deployment allowance processing. Request Id: {allowanceTrackerVM.RequestId}");

                string officerOperativeDirectCostPercentage = ConfigurationManager.AppSettings["PSSOfficerAllowanceDeduction"];
                if (string.IsNullOrEmpty(officerOperativeDirectCostPercentage))
                {
                    log.Error($"Allowance payment configuration was not found. Request Id: {allowanceTrackerVM.RequestId}");
                    throw new Exception("Allowance payment configuration was not found");
                }

                bool parsed = decimal.TryParse(officerOperativeDirectCostPercentage, out decimal convertedOfficerOperativeDirectCostPercentage);
                if (!parsed)
                {
                    log.Error($"Unable to convert configured operative direct cost deduction percentage. Request Id: {allowanceTrackerVM.RequestId}");
                    throw new Exception("Unable to convert configured operative direct cost deduction percentage.");
                }

                PoliceOfficerDeploymentAllowanceVM deploymentAllowance = new PoliceOfficerDeploymentAllowanceVM();
                List<DeploymentAllowanceItemVM> deploymentAllowances = new List<DeploymentAllowanceItemVM> { };

                //SettlementCycleEndDate substracted by SettlementCycleStartDate plus a day because start and end date are inclusive
                //e.g 31-01-2022 - 01-01-2022 will give 30 days without the addition of one day
                int numberOfDays = (allowanceTrackerVM.SettlementCycleEndDate - allowanceTrackerVM.SettlementCycleStartDate).Days + 1;
                foreach (PoliceOfficerDeploymentVM officer in officers)
                {
                    decimal invoiceContributedAmount = Math.Round(officer.DeploymentRate * numberOfDays, 2, MidpointRounding.ToEven);

                    decimal paymentPercentage = 100m; //This represent the percentage of the fee to pay the officer after direct cost has been deducted from the whole amount
                    string narration = $"100% monthly fee for {officer.OfficerName} for duration {allowanceTrackerVM.SettlementCycleStartDate.ToString("dd/MM/yyyy")} to {allowanceTrackerVM.SettlementCycleEndDate.ToString("dd/MM/yyyy")}";

                    log.Info($"Monthly fee processing ::: {narration}. Number of days::{numberOfDays}. . Request Id: {allowanceTrackerVM.RequestId}");

                    DeploymentAllowanceItemVM deploymentLog = new DeploymentAllowanceItemVM
                    {
                        Status = (int)DeploymentAllowanceStatus.PendingApproval,
                        PoliceOfficerLogId = officer.PoliceOfficerLogId,
                        Amount = ComputeAllowanceFee(invoiceContributedAmount, convertedOfficerOperativeDirectCostPercentage, paymentPercentage),
                        ContributedAmount = invoiceContributedAmount,
                        Narration = narration,
                        RequestId = allowanceTrackerVM.RequestId,
                        InvoiceId = allowanceTrackerVM.InvoiceId,
                        PaymentStageId = (int)PSSAllowancePaymentStage.MonthlyFee,
                        CommandId = officer.CommandId,
                        EscortDetailsId = allowanceTrackerVM.EscortDetailId
                    };
                    deploymentAllowances.Add(deploymentLog);
                }
                deploymentAllowance.AllowanceItems = deploymentAllowances;
                deploymentAllowance.DeploymentAllowanceTrackerId = allowanceTrackerVM.Id;

                if (allowanceTrackerVM.EscortEndDate == allowanceTrackerVM.NextSettlementDate)
                {
                    //If the request end date is equal to current NextSettlementDate
                    //NextSettlementDate, NextSettlementCycleStartDate and NextSettlementCycleEndDate will remain the same
                    deploymentAllowance.IsSettlementCompleted = true;
                    deploymentAllowance.NumberOfSettlementDone = allowanceTrackerVM.NumberOfSettlementDone + 1;
                    deploymentAllowance.NextSettlementDate = allowanceTrackerVM.NextSettlementDate;
                    deploymentAllowance.NextSettlementCycleStartDate = allowanceTrackerVM.SettlementCycleStartDate;
                    deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.SettlementCycleEndDate;
                }
                else if (allowanceTrackerVM.EscortEndDate <= allowanceTrackerVM.NextSettlementDate.AddMonths(1))
                {
                    //If the remaining duration is less than or equal to a month by comparing if request end date is less than or equal current NextSettlementDate plus a month
                    //Set SettlementCycleStartDate to current NextSettlementDate plus a day, then SettlementCycleEndDate to request end date, and NextSettlementDate to the request end date
                    deploymentAllowance.NumberOfSettlementDone = allowanceTrackerVM.NumberOfSettlementDone + 1;
                    deploymentAllowance.NextSettlementDate = allowanceTrackerVM.EscortEndDate;
                    deploymentAllowance.NextSettlementCycleStartDate = allowanceTrackerVM.NextSettlementDate.AddDays(1);
                    deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.EscortEndDate;
                }
                else if (allowanceTrackerVM.EscortEndDate > allowanceTrackerVM.NextSettlementDate.AddMonths(1))
                {
                    //If the remaining duration is more than a month by comparing if request end date is more than current NextSettlementDate plus a month
                    //Set SettlementCycleStartDate to current NextSettlementDate plus a day, then SettlementCycleEndDate to current NextSettlementDate plus a month, and NextSettlementDate to current NextSettlementDate plus a month
                    deploymentAllowance.NumberOfSettlementDone = allowanceTrackerVM.NumberOfSettlementDone + 1;
                    deploymentAllowance.NextSettlementCycleStartDate = allowanceTrackerVM.NextSettlementDate.AddDays(1);

                    //Check if the current NextSettlementDate is the last day of the month
                    //If yes, NextSettlementDate and NextSettlementCycleEndDate need to be end on the following month
                    //E.g if current settlement date is 30/04/2022 by adding a month it will give 30/05/2022, but we need 31/05/2022
                    int numberOfDaysInMonth = DateTime.DaysInMonth(allowanceTrackerVM.NextSettlementDate.Year, allowanceTrackerVM.NextSettlementDate.Month);
                    if (numberOfDaysInMonth == allowanceTrackerVM.NextSettlementDate.Day)
                    {
                        int nextMonthNumberOfDays = DateTime.DaysInMonth(allowanceTrackerVM.NextSettlementDate.AddDays(1).Year, allowanceTrackerVM.NextSettlementDate.AddDays(1).Month);
                        deploymentAllowance.NextSettlementDate = allowanceTrackerVM.NextSettlementDate.AddDays(nextMonthNumberOfDays);
                        deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.NextSettlementDate.AddDays(nextMonthNumberOfDays);
                    }
                    else
                    {
                        deploymentAllowance.NextSettlementDate = allowanceTrackerVM.NextSettlementDate.AddMonths(1);
                        deploymentAllowance.NextSettlementCycleEndDate = allowanceTrackerVM.NextSettlementDate.AddMonths(1);
                    }
                }
                return deploymentAllowance;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Compute deployment allowance
        /// </summary>
        /// <param name="invoiceContributedAmount"></param>
        /// <param name="deductionPercentage"></param>
        /// <param name="paymentPercentage"></param>
        /// <returns></returns>
        private decimal ComputeAllowanceFee(decimal invoiceContributedAmount, decimal deductionPercentage, decimal paymentPercentage)
        {
            //Get the percentage to be deducted for officer allowance
            decimal deductionAmount = Math.Round(((deductionPercentage / 100) * invoiceContributedAmount), 2, MidpointRounding.ToEven);

            //Get the payment allowance
            return Math.Round(((paymentPercentage / 100) * deductionAmount), 2, MidpointRounding.ToEven);
        }

        private void StartHangfireServer()
        {
            var conStringName = ConfigurationManager.AppSettings["HangfireConnectionStringName"];

            if (string.IsNullOrEmpty(conStringName))
            {
                throw new Exception("Unable to get the hangfire connection string name");
            }

            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringName);

            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }
    }
}
