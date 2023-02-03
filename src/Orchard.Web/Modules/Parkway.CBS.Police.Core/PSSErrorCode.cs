namespace Parkway.CBS.Police.Core
{
    /// <summary>
    /// Error Codes
    /// </summary>
    public enum PSSErrorCode
    {
        /// <summary>
        /// PSS Request not pending approval
        /// </summary>
        PSSRNPA,

        /// <summary>
        /// PSS Request not approved yet
        /// </summary>
        PSSRNA,

        /// <summary>
        /// PSS Biometric App is outdated
        /// </summary>
        PSSBAIO = 420,
    }
}