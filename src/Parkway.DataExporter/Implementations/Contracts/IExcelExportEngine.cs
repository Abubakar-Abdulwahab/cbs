using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations.Contracts
{
    public interface IExcelExportEngine
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Export(object data);

        /// <summary>
        /// This is a copy of excel generator that saves the generated excel at a specified location
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSavingPath"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        string SaveAsExcel<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath);


        /// <summary>
        /// This is a copy of excel generator that can be used to download a copy of the file through broswer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSavingPath"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        byte[] DownloadAsExcel<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath);


        /// <summary>
        /// This is a copy of async excel generator that can be used to download a copy of the file through broswer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSavingPath"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        Task<string> SaveAsExcelAsync<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath);


        /// <summary>
        /// This is a copy of excel generator that can be used to download a copy of the file through broswer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSavingPath"></param>
        /// <param name="resourcesRootPath"></param>
        /// <returns></returns>
        Task<byte[]> DownloadAsExcelAsync<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath);


    }
}
