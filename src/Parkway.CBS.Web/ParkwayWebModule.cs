using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.Web
{
    class ParkwayWebModule : IHttpModule
    {

        HttpApplication _context;

        public void Init(HttpApplication context)
        {
            _context = context;
            //context.BeginRequest += (new EventHandler(this.Application_BeginRequest));
            context.Error += (new EventHandler(this.HandleErrors));
        }

        public void Dispose() { }

        private void HandleErrors(object sender, EventArgs e)
        {
            HttpContext senderContext = ((HttpApplication)sender).Context;
            var exception = _context.Server.GetLastError();
            if (exception.Message == "Maximum request length exceeded.")
            {
                _context.Server.ClearError();
                _context.Response.ClearContent();
                //senderContext.Items.Add("Error", "Maximum file size exceeded.");
                //senderContext.Response.Redirect("~/");
                //_context.Server.Transfer("~/");
                if (_context.Session["Error"] == null) { _context.Session.Add("Error", "Maximum file size exceeded."); }
                else { _context.Session["Error"] = "Maximum file size exceeded."; }
                _context.Response.Redirect("~/", true);
            }
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            // Create HttpApplication and HttpContext objects to access
            // request and response properties.
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            //context.Response.Write("<h1><font color=red>HelloWorldModule: Beginning of Request</ font ></ h1 >< hr > ");
        }
    }
}