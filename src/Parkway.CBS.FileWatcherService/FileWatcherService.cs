using log4net;
using Parkway.CBS.PayeeProcessor.Utils;
using Parkway.CBS.PayeeProcessor.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parkway.CBS.FileWatcherService
{
    partial class FileWatcherService : ServiceBase
    {
        static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FileWatcherService()
        { 
            Run(); 
        }


        public void Run()
        {
            try
            {
                var directoriestoWatch = Utility.GetDirectorysToWatch();

                var list = new List<string>();

                if (directoriestoWatch != null)
                {
                    foreach (var directorytowatch in directoriestoWatch)
                    {
                        list.Add(directorytowatch);
                    }
                }

                foreach (var path in list)
                {
                    Logger.Info($"Watching folder path {path}");
                    Watch(path);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("An Error occurred while initializing FileWatcher Service to watch", ex);
            }
            
             
        }

        public void Watch(string watch_folder)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = watch_folder;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.xlsx";
            

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(NewFile_Created);
            watcher.Created += new FileSystemEventHandler(NewFile_Created);
            watcher.Deleted += new FileSystemEventHandler(File_Deleted);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
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

                Logger.InfoFormat($"New file with name {e.Name} detected in folder {e.FullPath}. ");

                StartFileProcessorTenantInstance(e.Name, e.FullPath);
            }, TaskCreationOptions.LongRunning);


        }

        private void StartFileProcessorTenantInstance(string fileName, string filePath)
        {
            try
            {
                var payeeProcessorFactory = new PayeeFileProcessorFactory();

                var configPath = Utility.FormatFileFullPath(fileName, filePath);

                Logger.Info($"Format file full path to get the configPath {configPath}");
              
                var fileprocessorinstance = Utility.GetPayeeFileProcessorConfig(configPath);
                if (fileprocessorinstance.UseDefault == true)
                {
                    var payeefileprocessor = payeeProcessorFactory.CreateInstance("DefaultPayeeFileProcessor");

                    payeefileprocessor.ProcessFile(fileName, filePath, fileprocessorinstance.Tenant.Name);
                }
                else
                {
                    var payeefileprocessor = payeeProcessorFactory.CreateInstance(fileprocessorinstance.Name);

                    payeefileprocessor.ProcessFile(fileName, filePath, fileprocessorinstance.Tenant.Name);
                     
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error occurred starting File Processor Tenant Instance.", ex);
            }
            
             
            
        }


        private void File_Renamed(object sender, FileSystemEventArgs e)
        {
            Logger.InfoFormat($"File Rename. {e.Name}");

        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
