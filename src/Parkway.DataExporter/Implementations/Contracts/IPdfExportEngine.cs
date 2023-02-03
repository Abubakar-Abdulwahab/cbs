using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations.Contracts
{
    public interface IPDFExportEngine
    {

        /// <summary>
        /// This is a copy of pdf generator that saves the generated pdf at a specified location
        /// </summary>
        /// <param name="cssPathArray"></param>
        /// <param name="templatePath"></param>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSavingPath"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        string SaveAsPdf(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath);


        /// <summary>
        /// This is a copy of pdf generator that can be used to download a copy of the file through broswer
        /// </summary>
        /// <param name="cssPathArray"></param>
        /// <param name="templatePath"></param>
        /// <param name="model"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        byte[] DownloadAsPdf(string[] cssPathArray, string templatePath, object model, string resourcesRootPath);



        /// <summary>
        /// This is a copy of async pdf generator that saves the generated pdf at a specified location
        /// </summary>
        /// <param name="cssPathArray"></param>
        /// <param name="templatePath"></param>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSavingPath"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        Task<string> SaveAsPdfAsync(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath);


        /// <summary>
        /// This is a copy of pdf generator that can be used to download a copy of the file through browser Asynchronously
        /// </summary>
        /// <param name="cssPathArray"></param>
        /// <param name="templatePath"></param>
        /// <param name="model"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        Task<byte[]> DownloadAsPdfAsync(string[] cssPathArray, string templatePath, object model, string resourcesRootPath);

        void SaveAsPdfNRecoLib(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath);

        void SaveAsPdfNRecoLibNoBorders(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath);

        byte[] DownloadAsPdfNRecoLib(string[] cssPathArray, string templatePath, object model, string resourcesRootPath);


        byte[] DownloadAsPdfNRecoLibNoBorders(string[] cssPathArray, string templatePath, object model, string resourcesRootPath);

    }
}
