using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class CustomerViewModel
    {
        public string Tin { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public DateTime DateCreated { get; set; }
       
    }    
}