using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class EmailNotificationModel
    {
        public string Subject { get; set; }

        public string Sender { get; set; }

        public string TemplateFileName { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public CBSUserVM CBSUser { get; set; }
    }
}