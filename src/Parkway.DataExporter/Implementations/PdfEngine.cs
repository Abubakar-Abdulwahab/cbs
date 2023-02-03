using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Parkway.DataExporter.Implementations.Contracts;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations
{
    public class PdfEngine : IPDFExportEngine
    {
        public string SaveAsPdf(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            try
            {

                string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);

                #region important variables

                byte[] pdf; // for holding the pdf byte
                string cssText = ""; // holds all the css text combined
                string html = ""; // holds the template after it gets filled with data 
                string css = ""; // holds css for each iteration
                string template = ""; // holds the template before it gets filled with data

                #endregion

                #region This obtains the template string from the specified path

                try
                {
                    //about to get the string from the template's virtual path or url
                    if (Uri.IsWellFormedUriString(templateFullPath, UriKind.Absolute))//the template is from a url
                    {
                        byte[] downloadedTemplateByte = new WebClient().DownloadData(templateFullPath);//RestSharp may be better for this job
                        string downloadedTemplateString = System.Text.Encoding.UTF8.GetString(downloadedTemplateByte);
                        template = downloadedTemplateString;
                    }
                    else
                    {
                        template = File.ReadAllText(templateFullPath);//the template is from a virtual path
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion

                #region Now we can add our model to the content of the template file (template string) we now have.

                try
                {
                    //Now we have the content of the template file, we can add our model to it.
                    if (templateFullPath.ToLower().EndsWith(".cshtml"))
                    {
                        //We'd be using RazorEngine
                        html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion

                #region copying the content of the css files.

                try
                {
                    if (cssPathArray != null)
                    {
                        foreach (var path in cssPathArray)
                        {
                            css = File.ReadAllText(path);
                            cssText += css;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                #endregion

                #region generating pdf byte from the given data

                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var document = new Document(PageSize.A4, 0, 0, 0, 0);
                        var writer = PdfWriter.GetInstance(document, memoryStream);
                        document.Open();
                        using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
                        {
                            using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream, System.Text.Encoding.UTF8, FontFactory.FontImp, resourcesRootPath);
                            }
                        }
                        document.Close();
                        pdf = memoryStream.ToArray();
                        string filePathForDownload = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("{0}/{1}/", resourcesRootPath, fileSavingPath));
                        Directory.CreateDirectory(filePathForDownload);

                        var target = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("{0}/{1}/{2}", resourcesRootPath, fileSavingPath, fileName));
                        File.WriteAllBytes(target, pdf);
                        return target;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion

            }
            catch (Exception ex)
            {
                #region what should happen in case of an error
                throw ex;
                #endregion
            }
        }

        public byte[] DownloadAsPdf(string[] cssPathArray, string templatePath, object model, string resourcesRootPath)
        {
            try
            {

                string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);
                #region important variables
                byte[] pdf; // for holding the pdf byte
                string cssText = ""; // holds all the css text combined
                string html = ""; // holds the template after it gets filled with data 
                string css = ""; // holds css for each iteration
                string template = ""; // holds the template before it gets filled with data
                #endregion

                #region This obtains the template string from the specified path
                try
                {
                    //about to get the string from the template's virtual path or url
                    if (Uri.IsWellFormedUriString(templateFullPath, UriKind.Absolute))//the template is from a url
                    {
                        byte[] downloadedTemplateByte = new WebClient().DownloadData(templateFullPath);
                        string downloadedTemplateString = System.Text.Encoding.UTF8.GetString(downloadedTemplateByte);
                        template = downloadedTemplateString;
                    }
                    else
                    {
                        template = File.ReadAllText(templateFullPath);//the template is from a virtual path
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion

                #region Now we can add our model to the content of the template file (template string) we now have.
                try
                {
                    //Now we have the content of the template file, we can add our model to it.
                    if (templateFullPath.ToLower().EndsWith(".cshtml"))
                    {
                        //We'd be using RazorEngine
                        html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion

                #region copying the content of the css files.
                try
                {
                    if (cssPathArray != null)
                    {
                        foreach (var path in cssPathArray)
                        {
                            css = File.ReadAllText(path);
                            cssText += css;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                #endregion

                #region generating pdf byte from the given data
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var document = new Document(PageSize.A4, 0, 0, 0, 0);
                        var writer = PdfWriter.GetInstance(document, memoryStream);
                        document.Open();
                        using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
                        {
                            using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream, System.Text.Encoding.UTF8, FontFactory.FontImp, resourcesRootPath);
                            }
                        }
                        document.Close();
                        pdf = memoryStream.ToArray();
                        return pdf;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion

            }
            catch (Exception ex)
            {
                #region what should happen in case of an error
                throw ex;
                #endregion
            }
        }

        public async Task<string> SaveAsPdfAsync(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            string target = string.Empty;


            await Task.Run(() =>
            {
                try
                {

                    string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);

                    #region important variables
                    byte[] pdf; // for holding the pdf byte
                    string cssText = ""; // holds all the css text combined
                    string html = ""; // holds the template after it gets filled with data 
                    string css = ""; // holds css for each iteration
                    string template = ""; // holds the template before it gets filled with data
                    #endregion

                    #region This obtains the template string from the specified path
                    try
                    {
                        //about to get the string from the template's virtual path or url
                        if (Uri.IsWellFormedUriString(templateFullPath, UriKind.Absolute))//the template is from a url
                        {
                            byte[] downloadedTemplateByte = new WebClient().DownloadData(templateFullPath);//RestSharp may be better for this job
                            string downloadedTemplateString = System.Text.Encoding.UTF8.GetString(downloadedTemplateByte);
                            template = downloadedTemplateString;
                        }
                        else
                        {
                            template = File.ReadAllText(templateFullPath);//the template is from a virtual path
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion

                    #region Now we can add our model to the content of the template file (template string) we now have.
                    try
                    {
                        //Now we have the content of the template file, we can add our model to it.
                        if (templateFullPath.ToLower().EndsWith(".cshtml"))
                        {
                            //We'd be using RazorEngine
                            html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    #endregion

                    #region copying the content of the css files.
                    try
                    {
                        if (cssPathArray != null)
                        {
                            foreach (var path in cssPathArray)
                            {
                                css = File.ReadAllText(path);
                                cssText += css;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    #endregion

                    #region generating pdf byte from the given data
                    try
                    {

                        using (var memoryStream = new MemoryStream())
                        {
                            var document = new Document(PageSize.A4, 0, 0, 0, 0);
                            var writer = PdfWriter.GetInstance(document, memoryStream);
                            document.Open();
                            using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
                            {
                                using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                                {
                                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream, System.Text.Encoding.UTF8, FontFactory.FontImp, resourcesRootPath);
                                }
                            }
                            document.Close();
                            pdf = memoryStream.ToArray();
                            target = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("{0}/{1}/{2}", resourcesRootPath, fileSavingPath, fileName));
                            File.WriteAllBytes(target, pdf);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    #region what should happen in case of an error
                    throw ex;
                    #endregion
                }
            });

            return target;

        }

        public async Task<byte[]> DownloadAsPdfAsync(string[] cssPathArray, string templatePath, object model, string resourcesRootPath)
        {
            return await ProcessPDF(cssPathArray, templatePath, model, resourcesRootPath);
        }

        public async Task<byte[]> ProcessPDF(string[] cssPathArray, string templatePath, object model, string resourcesRootPath)
        {
            string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);

            byte[] response = await Task.Run(() =>
            {
                try
                {

                    #region important variables
                    byte[] pdf; // for holding the pdf byte
                    string cssText = ""; // holds all the css text combined
                    string html = ""; // holds the template after it gets filled with data 
                    string css = ""; // holds css for each iteration
                    string template = ""; // holds the template before it gets filled with data
                    #endregion

                    #region This obtains the template string from the specified path
                    try
                    {
                        //about to get the string from the template's virtual path or url
                        if (Uri.IsWellFormedUriString(templateFullPath, UriKind.Absolute))//the template is from a url
                        {
                            byte[] downloadedTemplateByte = new WebClient().DownloadData(templateFullPath);
                            string downloadedTemplateString = System.Text.Encoding.UTF8.GetString(downloadedTemplateByte);
                            template = downloadedTemplateString;
                        }
                        else
                        {
                            template = File.ReadAllText(templateFullPath);//the template is from a virtual path
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion

                    #region Now we can add our model to the content of the template file (template string) we now have.
                    try
                    {
                        //Now we have the content of the template file, we can add our model to it.
                        if (templateFullPath.ToLower().EndsWith(".cshtml"))
                        {
                            //We'd be using RazorEngine
                            html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    #endregion

                    #region copying the content of the css files.
                    try
                    {
                        if (cssPathArray != null)
                        {
                            foreach (var path in cssPathArray)
                            {
                                css = File.ReadAllText(path);
                                cssText += css;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    #endregion

                    #region generating pdf byte from the given data
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var document = new Document(PageSize.A4, 0, 0, 0, 0);
                            var writer = PdfWriter.GetInstance(document, memoryStream);
                            document.Open();
                            using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
                            {
                                using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                                {
                                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream, System.Text.Encoding.UTF8, FontFactory.FontImp, resourcesRootPath);
                                }
                            }
                            document.Close();
                            pdf = memoryStream.ToArray();
                            return pdf;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    #region what should happen in case of an error
                    throw ex;
                    #endregion
                }
            });

            return response;
        }


        public void SaveAsPdfNRecoLib(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            try
            {
                string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);
                string savingPath = System.Web.HttpContext.Current.Server.MapPath(fileSavingPath);
                Directory.CreateDirectory(savingPath);

                //Convert to html
                string template = File.ReadAllText(templateFullPath);//the template is from a virtual path
                string html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.GeneratePdf(html, null, savingPath + "/" + fileName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public void SaveAsPdfNRecoLibNoBorders(string[] cssPathArray, string templatePath, object model, string fileName, string fileSavingPath, string resourcesRootPath)
        {
            try
            {
                string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);
                string savingPath = System.Web.HttpContext.Current.Server.MapPath(fileSavingPath);
                Directory.CreateDirectory(savingPath);

                //Convert to html
                string template = File.ReadAllText(templateFullPath);//the template is from a virtual path
                string html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
                htmlToPdf.Margins = new NReco.PdfGenerator.PageMargins
                {
                    Top = 0,
                    Left = 0,
                    Right = 0,
                    Bottom = 0,
                };
                htmlToPdf.GeneratePdf(html, null, savingPath + "/" + fileName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public byte[] DownloadAsPdfNRecoLib(string[] cssPathArray, string templatePath, object model, string resourcesRootPath)
        {
            try
            {
                string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);

                //Convert to html
                string template = File.ReadAllText(templateFullPath);//the template is from a virtual path

                //.replace is a temporary solution to remove the model name currently being showing on the receipt page
                string html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                return htmlToPdf.GeneratePdf(html, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] DownloadAsPdfNRecoLibNoBorders(string[] cssPathArray, string templatePath, object model, string resourcesRootPath)
        {
            try
            {
                string templateFullPath = System.Web.HttpContext.Current.Server.MapPath(resourcesRootPath + templatePath);

                //Convert to html
                string template = File.ReadAllText(templateFullPath);//the template is from a virtual path

                //.replace is a temporary solution to remove the model name currently being showing on the receipt page
                string html = Engine.Razor.RunCompile(template, DateTime.Now.Ticks.ToString(), null, model);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
                htmlToPdf.Margins = new NReco.PdfGenerator.PageMargins
                {
                    Top = 0,
                    Left = 0,
                    Right = 0,
                    Bottom = 0,
                };
                return htmlToPdf.GeneratePdf(html, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
