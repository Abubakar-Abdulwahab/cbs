using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class FileUploadTemplatesVM
    {
        public List<TemplateVM> ListOfTemplates { get; set; }

        public string SelectedTemplate { get; set; }

        public string SelectedImplementation { get; set; }
    }

    public class TemplateVM
    {
        public List<UploadImplInterfaceVM> ListOfUploadImplementations { get; set; }

        public string Name { get; set; }
    }

    public class UploadImplInterfaceVM
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}