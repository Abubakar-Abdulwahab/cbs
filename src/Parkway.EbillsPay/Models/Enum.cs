using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay.Models
{
    public enum EnvValues
    {
        None,
        ErrorCode = 999,
        FormatError = 30,
        SessionID404 = 15,
        BankCode404 = 16,
        OK = 00,
        Record404 = 25,
        /// <summary>
        /// System malfunction
        /// </summary>
        InternalError = 96,
        BillerMismatch = 03,
        ProductIdMismatch = 673,
        SecurityViolation = 63,
    }

    public enum EnvKeys
    {
        EbillsBillerID,
        EbillsBillerName,
        EbillsProductID,
        EbillsProductName,
        NibssResetEmail
    }

}
