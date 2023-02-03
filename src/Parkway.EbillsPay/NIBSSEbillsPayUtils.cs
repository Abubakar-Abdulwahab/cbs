using Parkway.EbillsPay.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay
{
    public class NIBSSEbillsPayUtils
    {
        public static string GetEnvValue(EnvValues key)
        {
            NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("NIBSSEBillsSettings");
            return section[key.ToString()];
        }
    }
}
