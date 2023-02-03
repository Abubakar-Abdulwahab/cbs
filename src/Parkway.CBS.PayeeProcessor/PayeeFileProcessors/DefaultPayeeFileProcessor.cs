using Parkway.CBS.PayeeProcessor.Contract;
using Parkway.CBS.PayeeProcessor.Utils;
using Parkway.CBS.PayeeProcessor.PayeeFileProcessorCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Parkway.CBS.Payee;
using Parkway.CBS.PayeeProcessor.DAL.Model;
using Parkway.CBS.PayeeProcessor.DAL.Repository;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;

namespace Parkway.CBS.PayeeProcessor.PayeeProcessors
{
    /// <summary>
    /// Default Implementation of the Payee File Processor
    /// </summary>
    public class DefaultPayeeFileProcessor: IBasePayeeFileProcessor
    {
        private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PayeeFileProcessor payeefileProcessorConfig;

        private string _tenantIdentifier;       

        public delegate void DatabaseProgressDelegate(long DAssBatchId, int totalNo, int count, decimal totalAmount);

        public event DatabaseProgressDelegate NewPayeeRecordSaved;


        /// <summary>
        /// Serves as a constructor to initialize required components.
        /// </summary>
        public void InitializeComponent()
        {
            payeefileProcessorConfig = Utility.GetPayeeFileProcessorConfigByTenantIdentifer(_tenantIdentifier);
            this.NewPayeeRecordSaved += UpdateDatabaseOfProgress;
        }


        /// <summary>
        /// Entry point to the default payee file Processor implementation to process file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileFullPath"></param>
        /// <param name="tenantIdentifier"></param>
        public void ProcessFile(string fileName, string fileFullPath, string tenantIdentifier)
        {
            _tenantIdentifier = tenantIdentifier;

            InitializeComponent();

            MoveFileToProcessingFolder(fileFullPath, fileName);
             
        }
         
        /// <summary>
        /// Moves file to Processing folder
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public void MoveFileToProcessingFolder(string filePath, string fileName)
        { 
            try
            {
                string sourceFile = filePath; 

                string destinationPath = payeefileProcessorConfig.Path.Processingpath;

                string destinationFile = $"{destinationPath}\\{fileName}";

                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.
                if (!System.IO.Directory.Exists(destinationPath))
                {
                    System.IO.Directory.CreateDirectory(destinationPath);
                }
                // To move a file or folder to a new location:
                System.IO.File.Move(sourceFile, destinationFile);

                ProcessPayeeFile(destinationFile, fileName);
                 

            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred", ex);
                return;
            }
        }


        /// <summary>
        /// Start the process of processing the Payee File
        /// </summary>
        /// <param name="processingfileFullPath"></param>
        /// <param name="fileName"></param>
        private void ProcessPayeeFile(string processingfileFullPath, string fileName)
        {
            List<PayeeAssessmentLineRecordModel> payees = new List<PayeeAssessmentLineRecordModel> { };
            GetPayeResponse fileresponse = new GetPayeResponse();
            var session = NHibernateSessionManager.GetSession(payeefileProcessorConfig.SessionFactory.Name, "DEFAULT");
 
            try
            {
                Logger.InfoFormat($"Reading the file {fileName} from the processing folder file {processingfileFullPath}");

                IDirectAssessmentPayee _dAssPayee = new DirectAssessmentPayee();
                fileresponse = _dAssPayee.ReadFile(processingfileFullPath, string.Empty, string.Empty);

                if (fileresponse.HeaderValidateObject.Error == true)
                { 
                    Logger.InfoFormat($"Excel File Headers has Error. See Message {fileresponse.HeaderValidateObject.ErrorMessage}");
                    //getting the bactch record from database
                    var directAssessmentrepository = new Repository<Parkway_CBS_Core_DirectAssessmentBatchRecord>(session); 
                    string payeefilePath = $"{payeefileProcessorConfig.Path.Directorytowatch}\\{fileName}"; 
                    Logger.InfoFormat($"Searching for DirectAssessment batch record with file path {payeefilePath}");

                    var record = directAssessmentrepository.Table.Where(c => c.FilePath == payeefilePath).SingleOrDefault(); 
                    record.ErrorOccurred = true;
                    record.ErrorMessage = fileresponse.HeaderValidateObject.ErrorMessage; 
                    directAssessmentrepository.Update(record);

                    Logger.InfoFormat($"Updated the Batch record for with error message {fileresponse.HeaderValidateObject.ErrorMessage}");

                }
                else
                {
                    payees = fileresponse.Payes;
                    ProcessExcelRecord(processingfileFullPath, fileName, payees);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Tried reading file from {0} folder but failed. See Error Message: {1}", processingfileFullPath, exception.Message), exception);
                return;
            }

        }

        /// <summary>
        /// Process the Excel File while in the processing folder
        /// </summary>
        /// <param name="processingfileFullPath"></param>
        /// <param name="fileName"></param>
        /// <param name="payees"></param>

        private void ProcessExcelRecord(string processingfileFullPath, string fileName, List<PayeeAssessmentLineRecordModel> payees)
        {

            var session = NHibernateSessionManager.GetSession(payeefileProcessorConfig.SessionFactory.Name, "DEFAULT");
            try
            {
                //getting the bactch record from database
                var directAssessmentrepository = new Repository<Parkway_CBS_Core_DirectAssessmentBatchRecord>(session);

                string payeefilePath = $"{payeefileProcessorConfig.Path.Directorytowatch}\\{fileName}";

                Logger.InfoFormat($"Retrieving batch record with file path {payeefilePath}");

                var queryDef = new List<KeyValuePair<string, object>>();
                queryDef.Add(new KeyValuePair<string, object>("FilePath", payeefilePath));

                var record = directAssessmentrepository.GetAllList(queryDef).SingleOrDefault();

                if (record == null)
                {
                    Logger.Error($"Cannot retrieve the Direct Assessment batch record attached for the file path {payeefilePath}");
                }
                else
                {
                    Logger.InfoFormat(string.Format("Processing payee Direct assessment for {0} and adapter {1}", record.FilePath, record.AdapterValue));
                }
                 
                AssessmentInterface adapter = GetDirectAssessmentAdapter(record.AdapterValue);

                Logger.InfoFormat(string.Format("Adapter Config retrieved {0}.", adapter.ClassName));
                IPayeeAdapter adpt = GetAdapterImplementation(adapter);
                Logger.InfoFormat(string.Format("Impl gotten for adatper {0} {1}. Getting the list of payee model", adapter.ClassName, adapter.Value));
                //var payees = adpt.GetPayeeModels(record.FilePath);
                Logger.InfoFormat(string.Format("Payee model got for {0} {1} {2}. Getting breakdown..", adapter.Value, adapter.ClassName, record.FilePath));
                var breakDown = adpt.GetRequestBreakDown(payees); 

                SavePayees(breakDown.Payees, record.Id, breakDown.TotalAmount);

                record.Amount = breakDown.TotalAmount;

                //move the files to a different location
                MoveFileToProcessedFolder(processingfileFullPath, fileName);

            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while saving payee record . See details of exception, {ex.Message}", ex);
            }
        }



        /// <summary>
        /// Perform the data transformation from Excel data to Payee Object and save the record in the database
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="Id"></param>
        /// <param name="totalAmount"></param>
        private void SavePayees(List<PayeeAssessmentLineRecordModel> payees, long Id, decimal totalAmount)
        {
            var session = NHibernateSessionManager.GetSession(payeefileProcessorConfig.SessionFactory.Name, "DEFAULT");
            var dAssPayRecRepository = new Repository<Parkway_CBS_Core_DirectAssessmentPayeeRecord>(session);

            var directAssessmentPayeeList = new List<Parkway_CBS_Core_DirectAssessmentPayeeRecord>();

            try
            {
                Logger.InfoFormat($"Transforming {payees.Count()} payee direct assessment record from the excel file to payee record");

                int processedrecordCount = 0;

                foreach (var payee in payees)
                { 
                    var payeeRecord = new Parkway_CBS_Core_DirectAssessmentPayeeRecord()
                    {
                        PayeeName = payee.TaxPayerName.Value,
                        GrossAnnual = payee.GrossAnnualEarnings.StringValue,
                        Exemptions = payee.Exemptions.StringValue,
                        TIN = payee.TaxPayerTIN.Value,
                        IncomeTaxPerMonth = payee.PayeeBreakDown.TaxStringValue,
                        IncomeTaxPerMonthValue = payee.PayeeBreakDown.Tax,
                        Month = payee.Month.StringValue,
                        Year = payee.Year.StringValue,
                        Email = payee.Email.Value,
                        Address = payee.Address.Value,
                        LGA = payee.LGA.Value,
                        PhoneNumber = payee.Phone.Value,
                        HasErrors = payee.HasError,
                        ErrorMessages = payee.ErrorMessages,
                        AssessmentDate = payee.AssessmentDate,
                        DirectAssessmentBatchRecord_Id = Id,
                    };
                    directAssessmentPayeeList.Add(payeeRecord);

                    processedrecordCount++;
                    //trigger an event to update database of percentage
                    //update the database for
                }



                dAssPayRecRepository.InsertRange(directAssessmentPayeeList);
                Logger.InfoFormat($"Total Number of Records processed {directAssessmentPayeeList.Count()}");

                NewPayeeRecordSaved(Convert.ToInt32(Id), payees.Count(), processedrecordCount, totalAmount);

            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while saving payee record to the database. See details of exception {ex.Message}", ex);
            }

        }

        /// <summary>
        /// Moves Processed Payee Excel File to the Processed Folder
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>

        public void MoveFileToProcessedFolder(string filePath, string fileName)
        {
            try
            {
                Logger.Info($"Finished processing file. Moving file {fileName} to processed folder"); 

                string sourceFile = filePath; 

                string destinationPath = payeefileProcessorConfig.Path.Processedpath;

                string destinationFile = $"{destinationPath}\\{fileName}";

                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.
                if (!System.IO.Directory.Exists(destinationPath))
                {
                    System.IO.Directory.CreateDirectory(destinationPath);
                }

                // To move a file or folder to a new location:
                System.IO.File.Move(sourceFile, destinationFile);
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred", ex);
                return;
            }

        }

        /// <summary>
        /// Update the Direct Assessment Batch Record upload record of the progress.
        /// </summary>
        /// <param name="DAssBatchId"></param>
        /// <param name="totalNo"></param>
        /// <param name="count"></param>
        /// <param name="totalAmount"></param>

        private void UpdateDatabaseOfProgress(long DAssBatchId, int totalNo, int count, decimal totalAmount)
        { 
            var session = NHibernateSessionManager.GetSession(payeefileProcessorConfig.SessionFactory.Name, "DEFAULT");

            var directAssessmentPayeeRepo = new Repository<Parkway_CBS_Core_DirectAssessmentBatchRecord>(session);

            var record = directAssessmentPayeeRepo.Find(DAssBatchId);

            int percentComplete = (int)Math.Round((double)(100 * count) / totalNo);
            //var percentageProgress = count  * 100 / totalNo; 
            record.PercentageProgress = percentComplete;
            record.TotalNoOfRowsProcessed = count;
            record.Amount = totalAmount;
            record.UpdatedAtUtc = DateTime.Now;
            Logger.InfoFormat($"Total amount for payee breakdown is {totalAmount}");
            Logger.Info($"Updating database of progress");
            directAssessmentPayeeRepo.Update(record);
        }

        /// <summary>
        /// Get the Direct Assessment Adapter
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <returns></returns>

        private AssessmentInterface GetDirectAssessmentAdapter(string adapterValue)
        {
            var session = NHibernateSessionManager.GetSession(payeefileProcessorConfig.SessionFactory.Name, "DEFAULT");
            Logger.InfoFormat("About to get CBS Tenant Details");
            var repository = new Repository<TenantCBSSettings>(session);
            TenantCBSSettings tenant = repository.Table.Where(c => c.Id != 0).FirstOrDefault();
            if (tenant == null) { throw new Exception("Tenant setting not found"); }
            return Utility.GetAssessmentType(adapterValue, tenant.Identifier);
        }


        /// <summary>
        /// Get the Direct Assessment Adapter Implementation
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        private IPayeeAdapter GetAdapterImplementation(AssessmentInterface adapter)
        {
            Logger.InfoFormat("Getting Adapter implementation..");
            var _directAssessmentPayee = new DirectAssessmentPayee();
            return _directAssessmentPayee.GetAdapter(adapter.ClassName);
        }

    }
}
