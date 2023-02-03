using ExcelDataReader;
using Hangfire;
using Parkway.CBS.ClientFileServices.Implementations.Contracts;
using Parkway.CBS.ClientFileServices.Implementations.IPPIS;
using Parkway.CBS.ClientFileServices.Implementations.Models;
using Parkway.CBS.ClientFileServices.Logger.Contracts;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.TaxPayerEnumerationService.Implementations;
using Parkway.CBS.TaxPayerEnumerationService.Implementations.Contracts;
using Parkway.CBS.TaxPayerEnumerationService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Parkway.CBS.ClientFileServices.Implementations.TaxPayerEnumeration
{
    public class TaxPayerEnumerationFileUpload : ITaxPayerEnumerationFileUpload
    {

        /// <summary>
        ///  Tax Payer Name - 0, 
        ///  Phone Number - 1,
        ///  Email - 2, 
        ///  TIN - 3, 
        ///  LGA - 4,
        ///  Address - 5,
        /// </summary>
        protected readonly string[] headerNames = { "Tax Payer Name".ToLower(), "Phone Number".ToLower(),
            "Email".ToLower(), "TIN".ToLower(), "LGA".ToLower(), "Address".ToLower() };

        private static readonly ILogger log = new Log4netLogger();

        public ITaxPayerEnumerationValidation _taxPayerEnumerationValidator { get; set; }

        public ITaxPayerEnumerationItemsDAOManager taxPayerEnumerationItemsDAO { get; set; }

        public ITaxPayerEnumerationDAOManager taxPayerEnumerationDAO { get; set; }

        public IUoW UoW { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "TaxPayerEnumerationExcelFileUploadProcessingJob");
            }
        }

        private void SetTaxPayerEnumerationDAOManager()
        {
            if (taxPayerEnumerationDAO == null) { taxPayerEnumerationDAO = new TaxPayerEnumerationDAOManager(UoW); }
        }

        private void SetTaxPayerEnumerationItemsDAOManager()
        {
            if (taxPayerEnumerationItemsDAO == null) { taxPayerEnumerationItemsDAO = new TaxPayerEnumerationItemsDAOManager(UoW); }
        }

        private void SetTaxPayerEnumerationValidationInstance()
        {
            if (_taxPayerEnumerationValidator == null) { _taxPayerEnumerationValidator = new TaxPayerEnumerationValidation(); }
        }


        /// <summary>
        /// Process tax payer enumeration line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="filePath"></param>
        /// <param name="tenantName"></param>
        public void ProcessTaxPayerEnumerationFileUpload(long batchId, string filePath, string tenantName)
        {
            //Commence hangfire processing
            try
            {
                var conStringNameKey = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HangfireConnectionStringName);
                if (string.IsNullOrEmpty(conStringNameKey))
                {
                    throw new Exception("Unable to get the hangfire connection string name");
                }
                StartHangfireServer(conStringNameKey);
                BackgroundJob.Enqueue(() => BeginProcessing(batchId, filePath, tenantName));

            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error while queuing enumeration batch excel file upload to Hangfire."));
                log.Error(exception.Message, exception);
                throw;
            }

        }

        private void StartHangfireServer(string conStringNameKey)
        {
            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringNameKey);
            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }


        /// <summary>
        /// Commence tax payer enumeration excel file processing
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="tenantName"></param>
        public void BeginProcessing(long batchId, string filePath, string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetTaxPayerEnumerationDAOManager();
                SetTaxPayerEnumerationItemsDAOManager();
                SetTaxPayerEnumerationValidationInstance();

                UoW.BeginTransaction();
                //Check if the enumeration batch has already been processed.
                var currentProccessStage = taxPayerEnumerationDAO.CheckTaxPayerEnumerationBatchStatus(batchId);
                if (currentProccessStage == TaxPayerEnumerationProcessingStages.Completed)
                { throw new InvalidOperationException($"Tax Payer Enumeration batch with id {batchId} processing has already passed this stage."); }
                //Get validated records from enumeration schedule excel file and save
                List<TaxPayerEnumerationLine> validatedLineItems = GetValidatedRecordsInScheduleFile(batchId, filePath);
                if (!validatedLineItems.Any()) { UoW.Commit(); return; }// if no record is returned the batch gets updated with a failed status and the background job terminates.
                taxPayerEnumerationItemsDAO.SaveEnumerationLineItemsRecords(validatedLineItems, batchId);

                //Update enumeration batch to reflect completion of records saved to table
                taxPayerEnumerationDAO.UpdateTaxPayerEnumerationBatchStatus(TaxPayerEnumerationProcessingStages.Completed, batchId);
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error($"Exception occured while trying to process tax payer enumeration schedule excel file records.");
                log.Error(exception.Message, exception);
                UoW.Rollback();
            }
        }


        /// <summary>
        /// Get validated records from tax payer enumeration schedule excel file.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<TaxPayerEnumerationLine> GetValidatedRecordsInScheduleFile(long batchId, string filePath)
        {

            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                { result = reader.AsDataSet(); }
            }
            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;

            Dictionary<string, HeaderValidationModel> headers = new Dictionary<string, HeaderValidationModel>
            {
                { headerNames[0], new HeaderValidationModel { } }, { headerNames[1], new HeaderValidationModel { } }, { headerNames[2], new HeaderValidationModel { } }, { headerNames[3], new HeaderValidationModel { } }, { headerNames[4], new HeaderValidationModel { } }, { headerNames[5], new HeaderValidationModel { } },
            };

            ValidateHeaders(rows[0], ref headers);
            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                taxPayerEnumerationDAO.UpdateTaxPayerEnumerationBatchStatus(TaxPayerEnumerationProcessingStages.Fail, batchId);
                return new List<TaxPayerEnumerationLine>();
            }

            rows.RemoveAt(0);

            var col = sheet1.Columns;
            List<TaxPayerEnumerationLine> validatedLineItems = new List<TaxPayerEnumerationLine> { };

            foreach (DataRow item in rows)
            {
                List<string> lineValues = new List<string>
                {
                    { item.ItemArray[headers[headerNames[0]].IndexOnFile].ToString()},//Tax Payer Name
                    { item.ItemArray[headers[headerNames[1]].IndexOnFile].ToString() }, //Phone Number
                    { item.ItemArray[headers[headerNames[2]].IndexOnFile].ToString() }, //Email
                    { item.ItemArray[headers[headerNames[3]].IndexOnFile].ToString() }, //TIN
                    { item.ItemArray[headers[headerNames[4]].IndexOnFile].ToString()}, //LGA
                    { item.ItemArray[headers[headerNames[5]].IndexOnFile].ToString()}, //Address
                };

                validatedLineItems.Add(_taxPayerEnumerationValidator.ValidateEnumerationLineItem(lineValues));
            }
            return _taxPayerEnumerationValidator.GetUniqueEnumerationItems(validatedLineItems);
        }


        /// <summary>
        /// Validate excel headers
        /// </summary>
        /// <param name="header"></param>
        /// <returns>HeaderValidateObject</returns>
        private void ValidateHeaders(DataRow header, ref Dictionary<string, HeaderValidationModel> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                if (headerNames[0].Contains(sItem)) { headers[headerNames[0]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[1].Contains(sItem)) { headers[headerNames[1]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[2].Contains(sItem)) { headers[headerNames[2]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[3].Contains(sItem)) { headers[headerNames[3]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[4].Contains(sItem)) { headers[headerNames[4]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[5].Contains(sItem)) { headers[headerNames[5]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
            }
        }
    }
}

