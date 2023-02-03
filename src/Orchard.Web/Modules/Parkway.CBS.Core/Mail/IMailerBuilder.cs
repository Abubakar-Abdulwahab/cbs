using Orchard;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Mail
{
    public interface IMailerBuilder : IDependency
    {
        /// <summary>
        /// Dictionary of emails. Key is the email address, Value is the Name of the recipient
        /// </summary>
        /// <param name="emails"></param>
        void To(Dictionary<string, string> emails);
        void From(string address="", string sender="");
        void Subject(string subject);
        void Body(string body, ListDictionary data);
        MailMessage GetMessage();
    }
}
