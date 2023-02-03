namespace Parkway.CBS.Core
{
    /// <summary>
    /// Error Codes
    /// </summary>
    public enum ErrorCode
    {
        PPNONE,
        /// <summary>
        /// invalid signature
        /// </summary>
        PPS1 = 0001,

        /// <summary>
        /// Internal server exception
        /// </summary>
        PPIE,

        /// <summary>
        /// User not found
        /// </summary>
        PPUSER404,

        /// <summary>
        /// invoicing error
        /// </summary>
        PPC1,

        /// <summary>
        /// cannot save mda record
        /// </summary>
        PPM1,

        /// <summary>
        /// MDA not found
        /// </summary>
        PPM404,

        /// <summary>
        /// no company key code found
        /// </summary>
        PPM3,

        /// <summary>
        /// Form validation error
        /// </summary>
        PPVE,

        /// <summary>
        /// Bank not found
        /// </summary>
        PPB1,

        /// <summary>
        /// Tenant not found
        /// </summary>
        PPTENANT404,        

        /// <summary>
        /// Cannot find revenue head
        /// </summary>
        PPRH404,

        /// <summary>
        /// Revenue head collection set up haas already been started
        /// </summary>
        PPR2,

        /// <summary>
        /// Cannot save revenue head exception
        /// </summary>
        PPR3,

        /// <summary>
        /// Already has billing. The revenue head already has billing information attached to it.
        /// </summary>
        PPB2,

        /// <summary>
        /// Client identity could not be found in the request header
        /// </summary>
        PPCK,

        /// <summary>
        /// No billing information found
        /// </summary>
        PPBILLING404,
        /// <summary>
        /// Billing not allowed, the revenue head or the mda is disabled/not active
        /// </summary>
        PPBILLINGNOTALLOWED,
        /// <summary>
        /// Billing type not found
        /// </summary>
        PPBILLINGTYPENOTFOUND,
        /// <summary>
        /// Billiing has ended
        /// </summary>
        PPBILLINGENDED,
        /// <summary>
        /// TIN not found
        /// </summary>
        PPTIN404,
        /// <summary>
        /// User not authorized
        /// </summary>
        PPUSER203,
        /// <summary>
        /// Caategoty not found
        /// </summary>
        PPCAT404,
        /// <summary>
        /// User already exists
        /// </summary>
        PPUSERALREADYEXISTS,
        /// <summary>
        /// user phone number already exists
        /// </summary>
        PPUSERPHONEALREADYEXISTS,
        /// <summary>
        /// Could not save tax entity record
        /// </summary>
        PPTAXENTITY500,
        /// <summary>
        /// Error saving user record
        /// </summary>
        PPUSERCBS500,
        /// <summary>
        /// No Record found
        /// </summary>
        PPREC404,
        PP_INVOICE_ALREADY_PAID = 1504,
        /// <summary>
        /// Error Code
        /// </summary>
        PPBCKCOLLECTERRCODE = 9999,

        /// <summary>
        /// Cannot find PAYE batch identifier
        /// </summary>
        PPPY404,

    }
}