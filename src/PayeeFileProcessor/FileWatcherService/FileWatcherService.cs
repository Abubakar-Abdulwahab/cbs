using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using PayeeProcessor;
using System.Threading;

namespace FileWatcherService
{
    public partial class FileWatcherService : ServiceBase
    {
         
        static Util util = new Util();
        static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  
        private readonly IPayeeFileProcessor _payeeFileProcessor;


        public FileWatcherService()
        {
            InitializeComponent();
           _payeeFileProcessor = new PayeeFileProcessor();
             
        }


        private void File_Deleted(object sender, FileSystemEventArgs e)
        {
            Logger.InfoFormat($"File {e.Name} Deleted from folder {e.FullPath}");
        }

        private void NewFile_Created(object sender, FileSystemEventArgs e)
        { 
            
            Task task = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2000);
                
                Logger.InfoFormat($"File is Ready, Moving File {e.Name} to Processing folder");
                 
                _payeeFileProcessor.MoveFileToProcessingFolder(e.FullPath, e.Name);
            }, TaskCreationOptions.LongRunning);
             
        }

        private void File_Renamed(object sender, FileSystemEventArgs e)
        {
            Logger.InfoFormat($"File Rename. {e.Name}");
            
        }


        public bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null; 
            Thread.Sleep(1000);

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);

               // stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
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
                
            }

            //file is not locked
            return false;
        }


        protected override void OnStart(string[] args)
        {

            Logger.InfoFormat("File Processor service is starting....");

            CBSFileWatcher.BeginInit();
            Logger.InfoFormat("Initializing file watcher...");
            CBSFileWatcher.Path = util.WatchedFolder;

            CBSFileWatcher.EndInit();
            Logger.InfoFormat($"Folder watcher initialized... watching { util.WatchedFolder} folder for payee excel records ");
             
        }

        protected override void OnStop()
        {
            Logger.InfoFormat($"Service Stopping");
            _payeeFileProcessor.Dispose();
        }
    }
}
