namespace Parkway.CBS.Core.HelperModels
{
    public class AdditionalDetails
    {

        /// <summary>
        /// Id of the form control you are saving
        /// <para>This is the id of on the form revenue head joiner table</para>
        /// </summary>
        public int ControlIdentifier { get; set; }

        /// <summary>
        /// Identifier name
        /// <para>Control Name, friendly name</para>
        /// </summary>
        public string IdentifierName { get; set; }

        /// <summary>
        /// Identifier value
        /// </summary>
        public string IdentifierValue { get; set; }

        /// <summary>
        /// Postion of the form in the list
        /// </summary>
        public int Index { get; internal set; }

    }
}