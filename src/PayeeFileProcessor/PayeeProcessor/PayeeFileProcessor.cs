using log4net;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using PayeeProcessor.DAL.Model;
using PayeeProcessor.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PayeeProcessor
{
    public interface IPayeeFileProcessor : IDisposable
    {
        void StartProcessingFile(string fileFullPath, string fileName);

        void MoveFileToProcessingFolder(string filePath, string fileName);

    }
    public class PayeeFileProcessor : IPayeeFileProcessor
    {
        private readonly IDirectAssessmentPayee _directAssessmentPayee;



        static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void DatabaseProgressDelegate(long DAssBatchId, int totalNo, int count, decimal totalAmount);

        public event DatabaseProgressDelegate NewPayeeRecordSaved;

        public PayeeFileProcessor()
        {
            _directAssessmentPayee = new DirectAssessmentPayee();

            this.NewPayeeRecordSaved += UpdateDatabaseOfProgress;
        }


        public void StartProcessingFile(string processingfileFullPath, string fileName)
        {
            List<PayeeAssessmentLineRecordModel> payees = new List<PayeeAssessmentLineRecordModel> { };
            try
            {
                try
                {
                    Logger.InfoFormat($"Reading the file {fileName} from the processing folder file {processingfileFullPath}");

                    IDirectAssessmentPayee _dAssPayee = new DirectAssessmentPayee();
                    payees = _dAssPayee.ReadFile(processingfileFullPath);
                }
                catch (Exception exception)
                {
                    Logger.Error(string.Format("Tried reading file from {0} folder but failed. See Error Message: {1}", processingfileFullPath, exception.Message), exception);
                    return;
                } 

              
                //getting the bactch record from database
                var directAssessmentrepository = new Repository<Parkway_CBS_Core_DirectAssessmentBatchRecord>();               

                string payeefilePath = $"{ConfigurationManager.AppSettings["PayeeFilePath"]}\\{fileName}";

                Logger.InfoFormat($"Searching for DirectAssessment batch record with file path {payeefilePath}");

                var record = directAssessmentrepository.Table.Where(c => c.FilePath == payeefilePath).SingleOrDefault();

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

        private void MoveFileToProcessedFolder(string filePath, string fileName)
        {
            try
            {
                Logger.Info($"Finished processing file. Moving file {fileName} to processed folder");

                string sourceFile = filePath;


                string destinationPath = ConfigurationManager.AppSettings["ProcessedFilePath"];

                string destinationFile = $"{ConfigurationManager.AppSettings["ProcessedFilePath"]}\\{fileName}";

                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.
                if (!System.IO.Directory.Exists(destinationPath))
                {
                    System.IO.Directory.CreateDirectory(destinationPath);
                }

                // To move a file or folder to a new location:
                System.IO.File.Move(sourceFile, destinationFile);
            }
            catch(Exception ex)
            {
                Logger.Error("An error occurred", ex);
                return;
            }
           
        }


        public void MoveFileToProcessingFolder(string filePath, string fileName)
        {

            try
            {
                string sourceFile = filePath;
                 
                string destinationPath = ConfigurationManager.AppSettings["ProcessingFilePath"];

                string destinationFile = $"{ConfigurationManager.AppSettings["ProcessingFilePath"]}\\{fileName}";

                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.
                if (!System.IO.Directory.Exists(destinationPath))
                {
                    System.IO.Directory.CreateDirectory(destinationPath);
                } 
                // To move a file or folder to a new location:
                System.IO.File.Move(sourceFile, destinationFile);

                ProcessFile(destinationFile, fileName);


            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred", ex);
                return;
            } 
        }


        private void ProcessFile(string filePath, string fileName)
        {
            
            Logger.InfoFormat($"Start processing the file {fileName}");
 
            new PayeeFileProcessor().StartProcessingFile(filePath, fileName);

        }


        public bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            Logger.InfoFormat($"Checking if file is {file.FullName} locked");

            Thread.Sleep(1000);

            try
            {
                //stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                Logger.Error("Error ocurred", ex);
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                Logger.Error("Inside the finally block");
            }

            //file is not locked
            return false;
        }



        /// <summary>
        /// Perform the data transformation from Excel data to Payee Object and save the record
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="Id"></param>
        private void SavePayees(List<PayeeAssessmentLineRecordModel> payees, long Id, decimal totalAmount)
        {
            var dAssPayRecRepository = new Repository<Parkway_CBS_Core_DirectAssessmentPayeeRecord>();

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
                        Month = payee.Month.Value,
                        Year = payee.Year.StringValue,
                        Email = payee.Email.Value,
                        Address = payee.Address.Value,
                        LGA = payee.LGA.Value,
                        PhoneNumber = payee.Phone.Value,
                        HasErrors = payee.HasError,
                        ErrorMessages = payee.ErrorMessages,
                        DirectAssessmentBatchRecord_Id = Id,
                    }; 
                    directAssessmentPayeeList.Add(payeeRecord);
                     
                    processedrecordCount++; 
                    //trigger an event to update database of percentage
                    //update the database for
                }



                dAssPayRecRepository.SaveBundle(directAssessmentPayeeList);
                Logger.InfoFormat($"Total Number of Records processed {directAssessmentPayeeList.Count()}");

                NewPayeeRecordSaved(Convert.ToInt32(Id), payees.Count(), processedrecordCount, totalAmount);

            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while saving payee record to the database. See details of exception {ex.Message}", ex);
            }

        }

        /// <summary>
        /// Update the database of the progress of saving files.
        /// </summary>
        /// <param name="DAssBatchId"></param>
        /// <param name="totalNo"></param>
        /// <param name="count"></param>
        private void UpdateDatabaseOfProgress(long DAssBatchId, int totalNo, int count, decimal totalAmount)
        {
             
            var directAssessmentPayeeRepo = new Repository<Parkway_CBS_Core_DirectAssessmentBatchRecord>();

            var record = directAssessmentPayeeRepo.Find(DAssBatchId); 

            int percentComplete = (int)Math.Round((double)(100 * count) / totalNo);
            //var percentageProgress = count  * 100 / totalNo; 
            record.PercentageProgress = percentComplete;
            record.TotalNoOfRowsProcessed = count;
            record.Amount = totalAmount;
            Logger.InfoFormat($"Total amount for payee breakdown is {totalAmount}");
            Logger.Info($"Updating database of progress");
            directAssessmentPayeeRepo.Update(record);
        }

        /// <summary>
        /// Get Direct Payee Assessment Adapter 
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <returns></returns>

        private AssessmentInterface GetDirectAssessmentAdapter(string adapterValue)
        {
            Logger.InfoFormat("About to get CBS Tenant Details");
            var repository = new Repository<TenantCBSSettings>();
            TenantCBSSettings tenant = repository.Table.Where(c => c.Id != 0).FirstOrDefault();
            if (tenant == null) { throw new Exception("Tenant setting not found"); }
            return GetAssessmentType(adapterValue, tenant.Identifier);
        }

        private IPayeeAdapter GetAdapterImplementation(AssessmentInterface adapter)
        {
            Logger.InfoFormat("Getting Adapter implementation..");
            return _directAssessmentPayee.GetAdapter(adapter.ClassName);
        }

        /// <summary>
        /// Get Assessment Type
        /// </summary>
        /// <param name="identifer"></param>
        /// <returns></returns>
        private List<AssessmentInterface> GetAssessmentTypes(string identifer)
        {
            Logger.InfoFormat("Getting Assessment Type");
            identifer = identifer.Trim().Split(' ')[0];

            var xmlstring = (ConfigurationManager.GetSection(typeof(PayeeAssessmentCollection).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(PayeeAssessmentCollection));
            PayeeAssessmentCollection tenantAssessmentDataCollection = new PayeeAssessmentCollection();
            using (StringReader reader = new StringReader(xmlstring))
            {
                tenantAssessmentDataCollection = (PayeeAssessmentCollection)serializer.Deserialize(reader);
            }
            if (tenantAssessmentDataCollection == null) { return new List<AssessmentInterface>(); }
            AssessmentInterfaceItem tenantAssessmentSection = tenantAssessmentDataCollection.AssessmentInterfaceItem.Where(item => item.Name == identifer + "_AssessmentCollection").FirstOrDefault();
            if (tenantAssessmentSection == null) { return new List<AssessmentInterface>(); }
            return tenantAssessmentSection.AssessmentInterface.Where(impl => impl.IsActive).Select(impl => impl).ToList();
        }

        /// <summary>
        /// Get Assessment adapter
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <param name="identifier"></param>
        /// <returns>AssessmentInterface | null</returns>
        private AssessmentInterface GetAssessmentType(string adapterValue, string identifier)
        {
            Logger.InfoFormat("Getting Assessment Type Adapter");
            var adapters = GetAssessmentTypes(identifier);
            return adapters.Where(adp => adp.Value == adapterValue.Trim()).FirstOrDefault();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PayeeFileProcessor() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
         
    }
}
