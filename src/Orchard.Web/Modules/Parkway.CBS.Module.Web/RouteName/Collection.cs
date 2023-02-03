namespace Parkway.CBS.Module.Web.RouteName
{
    public static class Collection
    {

        /// <summary>
        /// "C.SelfAssessment"
        /// </summary>
        public static string GenerateInvoice
        {
            get { return "C.SelfAssessment"; }
        }


        /// <summary>
        ///  "C."
        /// </summary>
        public static string ClientPrefix
        {
            get { return "C."; }
        }


        /// <summary>
        /// C.Invoice.ThirdParty.Redirect
        /// </summary>
        public static string ThirdPartyRedirect { get { return "C.Invoice.ThirdParty.Redirect"; } }


        /// <summary>
        /// C.Make.Payment
        /// </summary>
        public static string MakePayment { get { return "C.MakePayment.Invoice"; } }

    }
}