using OfficeOpenXml;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Exceptions;
using Parkway.DataExporter.Implementations.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations
{
    public class ExcelEngine : IExcelExportEngine
    {

        IDictionary<string, string> headers;

        public ExcelEngine(IDictionary<string, string> headers)
        {
            Guard.Against(headers == null || !headers.Any(), "parameter {headers} not specified");
            this.headers = headers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Export(object data)
        {

            byte[] resp = null;
            try
            {
                var model = ((IEnumerable<IDictionary<string, string>>)data).ToList();

                var prefixedColumns = model.SelectMany(x => x).Where(x => x.Value.Trim().StartsWith("0")).Select(x => x.Key).Distinct();

                var colCount = headers.Count;
                var rowCount = model.Count;

                ExcelPackage package = new ExcelPackage();

                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Sheet1");

                var table = new DataTable();
                headers.ToList().ForEach(x => table.Columns.Add(x.Value));

                model.ForEach(x =>
                {
                    var row = table.NewRow();
                    headers.ToList().ForEach(z =>
                    {
                        var value = "";
                        x.TryGetValue(z.Key, out value);
                        row[z.Value] = value;
                    });
                    table.Rows.Add(row);
                });


                sheet.Cells.LoadFromDataTable(table, true);


                sheet.Cells[1, 1, rowCount, colCount].AutoFitColumns();
                sheet.Cells[1, 1, 1, colCount].Style.Font.Bold = true;

                resp = package.GetAsByteArray();

            }
            catch (Exception ex)
            {
            }

            return resp;
        }


        /// <summary>
        /// This is responsible for creating the excel file using the headers parameters and the data model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        static byte[] Export<T>(IDictionary<string, string> headers, IEnumerable<T> model)
        {
            var exporter = new ExcelEngine(headers);
            List<IDictionary<string, string>> data = new List<IDictionary<string, string>> {  };
            if (model.Any())
            {
                data = model.Select(x => x.ToDictionary(headers.Select(z => z.Key))).ToList();
            }
            else
            {
                IDictionary<string, string> emptyDcitionary = new Dictionary<string, string> { };
                foreach (var item in headers)
                {
                    emptyDcitionary.Add(item.Key, string.Empty);
                }
                data.Add(emptyDcitionary);
            }

            return exporter.Export(data);
        }

        /// <summary>
        /// This is a copy of excel generator implemented using Bank3D approach 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string SaveAsExcel<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            try
            {
                var data = Export<T>(headers, model);
                var target = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("{0}/{1}/{2}", resourcesRootPath, fileSavingPath, fileName));
                File.WriteAllBytes(target, data);
                return target;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public async Task<string> SaveAsExcelAsync<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            string target = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    var data = Export<T>(headers, model);
                    target = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("{0}/{1}/{2}", resourcesRootPath, fileSavingPath, fileName));
                    File.WriteAllBytes(target, data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return target;

        }

        public byte[] DownloadAsExcel<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            try
            {
                var data = Export<T>(headers, model);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<byte[]> DownloadAsExcelAsync<T>(IDictionary<string, string> headers, IEnumerable<T> model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            byte[] response = null;
            await Task.Run(() =>
            {
                try
                {
                    var data = Export<T>(headers, model);
                    response = data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return response;

        }
    }
}
