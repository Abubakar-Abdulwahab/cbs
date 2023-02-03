namespace Parkway.CBS.Core.Lang
{
    public static class ResponseCodeLang
    {
        /// <summary>
        /// 0000
        /// </summary>
        public static string ok { get { return "0000"; } }

        /// <summary>
        /// 1001
        /// <para>this return the code for when payment has already been processed</para>
        /// </summary>
        public static object payment_already_processed { get { return "1001"; } }


        /// <summary>
        /// 0001
        /// </summary>
        public static string could_not_compute_signature { get { return "0001"; } }

        /// <summary>
        /// 0002
        /// </summary>
        public static string user_not_authorized { get { return "0002"; } }

        /// <summary>
        /// 0003
        /// </summary>
        public static string payment_provider_404 { get { return "0003"; } }


        /// <summary>
        /// 1400
        /// </summary>
        public static string invoice_already_paid_for { get { return "1400"; } }

        /// <summary>
        /// 1404
        /// </summary>
        public static string invoice_404 { get { return "1404"; } }

        /// <summary>
        /// No part payments allowed for this invoice
        /// </summary>
        public static string no_part_payments_allowed { get { return "1405"; } }

        /// <summary>
        /// 9404
        /// </summary>
        public static string record_404 { get { return "9404"; } }


        /// <summary>
        /// 9999
        /// </summary>
        public static string generic_exception_code { get { return "9999"; } }

        /// <summary>
        /// 1500
        /// </summary>
        public static string tenant_404 { get { return "1500"; } }

        /// <summary>
        /// 1001
        /// <para>this return the code for when payment reference and invoice mismatch</para>
        /// </summary>
        public static string payment_data_mismatch { get { return "1002"; } }

    }
}