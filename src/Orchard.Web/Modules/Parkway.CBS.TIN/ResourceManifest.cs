using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.TIN
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("BootstrapCss").SetUrl("bootstrap.min.css");
            manifest
               .DefineStyle("CBS.Style.Main")
               .SetUrl("cbs-style-main.css");

            manifest.DefineStyle("Registration").SetUrl("Registration.css").SetDependencies("jQueryUI", "BootstrapCss");
            manifest.DefineStyle("FormWizard").SetUrl("FormWizard.css");
            manifest.DefineStyle("font-awesome").SetUrl("font-awesome.css");
            manifest.DefineStyle("DateTimePicker").SetUrl("DateTimePicker.css").SetDependencies("Bootstrap");

            manifest.DefineScript("jQuery").SetUrl("jquery-1.10.2.min.js");
            manifest.DefineScript("BootStrap").SetUrl("bootstrap.min.js");

            manifest.DefineScript("Toastr").SetUrl("toastr.min.js");
            manifest.DefineScript("Plugin").SetUrl("plugin.js");
            manifest.DefineScript("DateTimePicker").SetUrl("DateTimePicker.js").SetDependencies("jQuery", "BootStrap");
            manifest.DefineScript("TINRegistration").SetUrl("TINRegistration.js");
            manifest.DefineScript("EregistryFormWizard").SetUrl("EregistryFormWizard.js").SetDependencies("Toastr", "jQuery");
            manifest.DefineScript("ReportsJs").SetUrl("Reports.js");
        }
    }
}