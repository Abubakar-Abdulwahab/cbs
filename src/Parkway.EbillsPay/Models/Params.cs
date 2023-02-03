﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay.Models
{
    public class Params
    {
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AmountToPay { get; set; }
    }
}
