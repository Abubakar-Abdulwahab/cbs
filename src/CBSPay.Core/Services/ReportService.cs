using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using RazorEngine;
using Antlr3.ST;
using Antlr3.ST.Language;
using RazorEngine.Templating;
using log4net;
using System.Reflection;
using System.Security.Policy;
using System.Net;
namespace CBSPay.Core.Services
{
    public class ReportService
    {
        ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public byte[] GeneratePdf(string[] cssPathArray, string templatePath, object model, string resourcesRootPath = "")
        {
            Logger.InfoFormat("Now in the GetPdf method with template path: [{0}]", templatePath);
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
                    if (Uri.IsWellFormedUriString(templatePath, UriKind.Absolute))//the template is from a url
                    {
                        Logger.Info($"The template path is a url given by: {templatePath}");
                        byte[] downloadedTemplateByte = new WebClient().DownloadData(templatePath);//RestSharp may be better for this job
                        string downloadedTemplateString = System.Text.Encoding.UTF8.GetString(downloadedTemplateByte);
                        template = downloadedTemplateString;
                        Logger.Info("Template string successfully downloaded!!");
                    }
                    else
                    {
                        Logger.InfoFormat("The template path is a virtual path given by: {0}", templatePath);
                        template = File.ReadAllText(templatePath);//the template is from a virtual path
                        Logger.Info("Template string successfully copied!!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("An error occured while trying to get the template string from the given path; Error Message:: {0}; StackTrace:: {1}", ex.Message, ex.StackTrace);
                    return null;
                }
                #endregion

                #region Now we can add our model to the content of the template file (template string) we now have.
                try
                {
                    //Now we have the content of the template file, we can add our model to it.
                    if (templatePath.ToLower().EndsWith(".cshtml"))
                    {
                        //We'd be using RazorEngine
                        Logger.Info("the template is a Razor template");
                        html = Engine.Razor.RunCompile(template, "key", null, model);
                        Logger.Info("The Razor template now holds data");
                    }
                    else //if (templatePath.ToLower().EndsWith(".st"))
                    {
                        List<KeyValuePair<string, string>> data = (List<KeyValuePair<string, string>>)model;
                        //StringTemplateGroup TemplateGroup = new StringTemplateGroup("group", "", typeof(TemplateLexer));
                        //var stringPath = templatePath.Remove(templatePath.Length - 3);
                        //StringTemplate htmlst = TemplateGroup.GetInstanceOf(stringPath);
                        var TempFolder = "~/Temp/";
                        if (!Directory.Exists(TempFolder))
                        {
                            var dir = Directory.CreateDirectory(TempFolder);
                        }
                        var TempFile = "~/Temp/temp.st";
                        if (File.Exists(TempFile))
                        {
                            File.Delete(TempFile);
                        }
                        File.WriteAllText(TempFile, template);
                        StringTemplateGroup TemplateGroup = new StringTemplateGroup("group", TempFolder, typeof(TemplateLexer));
                        StringTemplate htmlst = TemplateGroup.GetInstanceOf("temp");
                        foreach (var pair in data)
                        {
                            htmlst.SetAttribute(pair.Key.ToString(), pair.Value.ToString());
                        }
                        html = htmlst.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("An error occured while trying to add our model to the content of the template file (template string) we have. Error Message:: {0}; Stack trace:: {1}", ex.Message, ex.StackTrace);
                    return null;
                }

                #endregion

                #region copying the content of the css files.
                try
                {
                    foreach (var path in cssPathArray)
                    {
                        css = File.ReadAllText(path);
                        cssText += css;
                    }
                }
                catch (Exception ex)
                {

                    Logger.ErrorFormat("An error occured while copying the content of the css files. Error Message:: {0}; Stack Trace:: {1}", ex.Message, ex.StackTrace);
                    return null;
                }
                #endregion

                #region generating pdf byte from the given data
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var document = new Document(PageSize.A4, 0, 0, 0, 0);//try 50,50,60,60
                        var writer = PdfWriter.GetInstance(document, memoryStream);
                        document.Open();
                        using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
                        {
                            using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                //XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream);
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
                    Logger.ErrorFormat("An error occured while generating pdf byte from the given data. Error Message:: {0}; Stack Trace:: {1}", ex.Message, ex.StackTrace);
                    return null;
                }
                #endregion

            }
            catch (Exception ex)
            {
                #region what should happen in case of an error
                Logger.ErrorFormat("an error occured while trying to generate pdf from the given input {0} ::: {1}", ex.Message, ex.StackTrace);
                return null;
                #endregion
            }
            #region other information
            //nb: this is still adapted to taking .st and .cstml files (hope to extend it to other text-like files e.g. json, xml, html,htm, txt etc.)
            //other parameters this method can hold includes content (and content-type), pageSize, coverImage, Border (border-color, border-thickness, water mark (text or image), back-ground-pattern (from an enum), pagenumber (pageNumberFormat))
            #endregion
        }
    }
}