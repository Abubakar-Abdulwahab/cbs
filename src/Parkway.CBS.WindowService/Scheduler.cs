using System;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parkway.CBS.WindowService.Configuration;
using Parkway.CBS.WindowService.Utility.FileWatcher;
using Parkway.CBS.WindowService.Utility.FileWatcher.Contracts;
using Parkway.CBS.ClientFileServices.Implementations.Contracts;
using Parkway.CBS.ClientFileServices.Implementations.IPPIS;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.ClientFileServices.Logger.Contracts;
using System.Threading;
using Parkway.CBS.ClientFileServices.Implementations.Models;

namespace Parkway.CBS.WindowsService
{
    public partial class Scheduler : ServiceBase
    {
        private static Dictionary<string, FileProcessor> fileProcessor;
        private static FileSystemWatcher _watcher;
        private static IFileWatcher _fileWatcher;
        private static readonly ILogger log = new Log4netLogger();


        /// <summary>
        /// Windows service
        /// <para>Instantiate all the instantiables you would need for this windows service</para>
        /// </summary>
        public Scheduler()
        {
            log.Info("Windows service started");

            InitializeComponent();

            #region File watcher section

            //instantiating dictionary to hold PayeeFileProcessor objs
            fileProcessor = new Dictionary<string, FileProcessor>();
            _fileWatcher = new FileWatcher();
            //gets all the file watcher configurations
            var getFileProcessor = _fileWatcher.GetFileProcessors();

            foreach (var fileProcessor in getFileProcessor)
            {
                //add to dictionary
                log.Info($"Watching the directory => {fileProcessor.Path.Directorytowatch} ");
                Directory.CreateDirectory(fileProcessor.Path.Directorytowatch);
                Scheduler.fileProcessor.Add(fileProcessor.Path.Directorytowatch, fileProcessor);
                ProcessRequest(fileProcessor.Path.Directorytowatch);
            }

            #endregion
        }

        protected override void OnStart(string[] args)
        {
            //logger.Info($"Service started");
        }

        protected override void OnStop()
        {
            //IPPISFileProcessor.log.Info($"Service stopped");
        }


        /// <summary>
        /// instantiate each file watcher entry, adding event handlers
        /// </summary>
        /// <param name="path"></param>
        public static void ProcessRequest(string path)
        {
            try
            {
                //File watcher filter can only watch a specific file type or watch all file types.
                // Empty string "" will watch all file types
                _watcher = new FileSystemWatcher();
                _watcher.Path = path;
                _watcher.Created += new FileSystemEventHandler(_watcher_Changed);
                _watcher.EnableRaisingEvents = true;
                _watcher.IncludeSubdirectories = true;
                _watcher.Filter = "";
            }
            catch (Exception ex)
            {
                log.Error("Error calling watcher event handler " + ex.Message + ex.StackTrace + ex.InnerException);
            }
        }


        /// <summary>
        /// When file has entered the directory, this method
        /// Gets the file full path and trim it
        /// Move the file to the processing folder
        /// Instantiate the implementing class and process the excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                //Trim the full file path to get the payee processor file path
                string path = _fileWatcher.TrimFilePath(e.FullPath, e.Name);
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(e.FullPath);
                log.Info($"New file created. Directory => {path} ");

                //get processor details
                FileProcessor fileProcessorDetails = fileProcessor[path];
                //check for null
                if (fileProcessorDetails == null)
                {
                    log.Info($"File processor details is null for path => {path} ");
                    throw new Exception("File processor details is null");
                }

                switch (fileProcessorDetails.ProcessorType)
                {
                    case "IPPIS":
                        IPPISFileProcessor(e.FullPath, fileNameWithoutExtension, fileProcessorDetails);
                        break;
                    case "DataEnumeration":
                        DataEnumerationFileProcessor(e.FullPath, fileNameWithoutExtension, fileProcessorDetails);
                        break;
                    case "IPPISSummary":
                        IPPISSummaryFileProcessor(e.FullPath, fileNameWithoutExtension, fileProcessorDetails);
                        break;
                    case "NAGIS":
                        NAGISDataFileProcessor(e.FullPath, fileNameWithoutExtension, fileProcessorDetails);
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception => {ex.Message + ex.StackTrace + ex.InnerException} ");
            }

        }


        /// <summary>
        /// This method handles IPPIS Summary file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="originalFileName"></param>
        /// <param name="fileProcessorDetails"></param>
        private static void IPPISSummaryFileProcessor(string fullPath, string originalFileName, FileProcessor fileProcessorDetails)
        {
            try
            {
                //Get file extension
                string extension = System.IO.Path.GetExtension(fullPath);

                if (extension.Equals(".xls") || extension.Equals(".xlsx"))
                {
                    string processingFilePath = MoveFile(fullPath, originalFileName, extension, fileProcessorDetails);
                    //this is the directory info of the file on the FTP location
                    var directoryInfo = new DirectoryInfo(fullPath);


                    log.Info($"File moved to the processing directory => {processingFilePath} ");
                    Task.Run(() =>
                    {
                        var nameSplit = fileProcessorDetails.ImplementationClass.ClassName.Split(',');

                        var implementingClass = Activator.CreateInstance(nameSplit[1].Trim(), nameSplit[0].Trim());
                        var impl = ((IFileWatcherProcessor)implementingClass.Unwrap());

                        var details = impl.GetFileServiceHelper(fileProcessorDetails.Tenant.Value, directoryInfo, fileProcessorDetails.Tenant.StateId, fileProcessorDetails.Tenant.UnknownTaxPayerCodeId, processingFilePath, fileProcessorDetails.Path.ProcessCSVPath, fileProcessorDetails.Path.Summarypath);
                        //, details.ProcessedSummaryCSVFilePath
                        impl.ConvertIPPISSummaryFileToCSV(details);
                    });
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception => {ex.Message + ex.StackTrace + ex.InnerException} ");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="originalFileName"></param>
        /// <param name="fileProcessorDetails"></param>
        public static void IPPISFileProcessor(string fullPath, string originalFileName, FileProcessor fileProcessorDetails)
        {
            try
            {
                //Get file extension
                string extension = System.IO.Path.GetExtension(fullPath);

                if (extension.Equals(".xls") || extension.Equals(".xlsx"))
                {

                    string processingFilePath = MoveFile(fullPath, originalFileName, extension, fileProcessorDetails);

                    //this is the directory info of the file on the FTP location
                    var directoryInfo = new DirectoryInfo(fullPath);


                    log.Info($"File moved to the processing directory => {processingFilePath} ");
                    Task.Run(() =>
                    {
                        var nameSplit = fileProcessorDetails.ImplementationClass.ClassName.Split(',');

                        var implementingClass = Activator.CreateInstance(nameSplit[1].Trim(), nameSplit[0].Trim());
                        var impl = ((IFileWatcherProcessor)implementingClass.Unwrap());

                        var details = impl.GetFileServiceHelper(fileProcessorDetails.Tenant.Value, directoryInfo, fileProcessorDetails.Tenant.StateId, fileProcessorDetails.Tenant.UnknownTaxPayerCodeId, processingFilePath, fileProcessorDetails.Path.ProcessCSVPath, fileProcessorDetails.Path.Summarypath);

                        impl.ValidateTheFile(details, processingFilePath);
                    });
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception => {ex.Message + ex.StackTrace + ex.InnerException} ");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="originalFileName"></param>
        /// <param name="fileProcessorDetails"></param>
        public static void DataEnumerationFileProcessor(string fullPath, string originalFileName, FileProcessor fileProcessorDetails)
        {
            try
            {
                //Get file extension
                string extension = System.IO.Path.GetExtension(fullPath);


                if (extension.Equals(".csv"))
                {
                    var fileNameSplit = originalFileName.Split('-');
                    long withHoldingTaxBatchId = 0;
                    Int64.TryParse(fileNameSplit[0].Trim(), out withHoldingTaxBatchId);

                    string processingFilePath = MoveFile(fullPath, originalFileName, extension, fileProcessorDetails);

                    //this is the directory info of the file on the FTP location
                    var directoryInfo = new DirectoryInfo(fullPath);


                    log.Info($"File moved to the processing directory => {processingFilePath} ");
                    Task.Run(() =>
                    {
                        IFileProcessor impl = new ClientFileServices.Implementations.FileProcessor();
                        impl.ProcessFile(fileProcessorDetails.Tenant.Value, processingFilePath, withHoldingTaxBatchId);
                    });
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception => {ex.Message + ex.StackTrace + ex.InnerException} ");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="originalFileName"></param>
        /// <param name="fileProcessorDetails"></param>
        public static void NAGISDataFileProcessor(string fullPath, string originalFileName, FileProcessor fileProcessorDetails)
        {
            try
            {
                //Get file extension
                string extension = System.IO.Path.GetExtension(fullPath);

                if (extension.Equals(".xls") || extension.Equals(".xlsx"))
                {
                    var fileNameSplit = originalFileName.Split('-');
                    long batchId = 0;
                    Int64.TryParse(fileNameSplit[0].Trim(), out batchId);

                    string processingFilePath = MoveFile(fullPath, originalFileName, extension, fileProcessorDetails);

                    //this is the directory info of the file on the FTP location
                    var directoryInfo = new DirectoryInfo(fullPath);


                    log.Info($"File moved to the processing directory => {processingFilePath} ");
                    Task.Run(() =>
                    {
                        var nameSplit = fileProcessorDetails.ImplementationClass.ClassName.Split(',');

                        var implementingClass = Activator.CreateInstance(nameSplit[1].Trim(), nameSplit[0].Trim());
                        var impl = ((IReferenceDataFileProcessor)implementingClass.Unwrap());
                        impl.SaveFile(fileProcessorDetails.Tenant.Value, processingFilePath, batchId);
                    });
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception => {ex.Message + ex.StackTrace + ex.InnerException} ");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="originalFileName"></param>
        /// <param name="fileProcessorDetails"></param>
        /// <returns></returns>
        public static string MoveFile(string fullPath, string originalFileName, string extension, FileProcessor fileProcessorDetails)
        {
            //Move the file to the processing folder
            string processingFilePath = fileProcessorDetails.Path.Processingpath + "\\" + originalFileName + DateTime.Now.ToString("dd-MM-yyyy HH mm ss") + extension;
            log.Info($"Processing directory => {processingFilePath} ");

            if (!Directory.Exists(fileProcessorDetails.Path.Processingpath))
            {
                Directory.CreateDirectory(fileProcessorDetails.Path.Processingpath);
            }

            bool movedFile = false;
            while (!movedFile)
            {
                try
                {
                    //Copy the file to the processed file directory
                    log.Info($"About to move file to the processing directory");
                    File.Move(fullPath, processingFilePath);
                    movedFile = true;
                }
                catch (Exception ex)
                {
                    //Logging this increases the size of the log
                    //Because it starts the attempt to move the file while it is still copying and 
                    // it will be getting file is being used by another process
                    //log.Error($"Exception => {ex.Message} ");
                }
            }

            return processingFilePath;
        }

    }
}
