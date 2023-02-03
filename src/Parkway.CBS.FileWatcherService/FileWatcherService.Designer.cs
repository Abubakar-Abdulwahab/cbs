using Parkway.CBS.PayeeProcessor.Utils;
using System.Collections.Generic;
using System;
using log4net;

namespace Parkway.CBS.FileWatcherService
{
    partial class FileWatcherService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
                
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

            this.CBSFileWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.CBSFileWatcher)).BeginInit();

            this.CBSFileWatcher.EnableRaisingEvents = true;

            this.CBSFileWatcher.NotifyFilter = ((System.IO.NotifyFilters)(((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName)
            | System.IO.NotifyFilters.Size)));
            this.CBSFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.NewFile_Created);
            this.CBSFileWatcher.Created += new System.IO.FileSystemEventHandler(this.NewFile_Created);
            this.CBSFileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.File_Deleted);
            this.CBSFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.File_Renamed);

             
            this.CBSFileWatcher.Filter = "*.xlsx";

            components = new System.ComponentModel.Container();

            this.ServiceName = "CBSPayeeFileWatcherService";

            
              
        }

        //private void Watch(string path)
        //{

            
        //}

        #endregion
        private System.IO.FileSystemWatcher CBSFileWatcher;
    }
}
