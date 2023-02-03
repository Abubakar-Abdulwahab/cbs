using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Helpers
{
    public enum CBSPayStatusCode
    {
        InvalidParameter = 190
    }

    public enum ButtonSize
    {
        Small,
        Medium,
        Large,
        Default
    }
    public enum PaymentChannel
    {
        [Description("BankCollect")]
        BankCollect = 1,
        [Description("PayDirect")]
        PayDirect = 2,
        [Description("QuickTeller")]
        NetPay = 3
    }

    /// <summary>
    /// EIRS Settlement Method as gotten via their API (13/06/2018)
    /// </summary>
    public enum EIRSSettlementMethod
    {
        [Description("Internet Web Pay")]
        InternetWebPay = 1,
        [Description("Bank Transfer")]
        BankTransfer = 2,
        [Description("Point of Sale (POS)")]
        PointofSale = 3,
        [Description("Mobile Payment")]
        MobilePayment = 4,
        [Description("Scratch Card Payment")]
        ScratchCardPayment = 5
    }

    public static class EnumHelper
    {
        public static string ToDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

    }

}
