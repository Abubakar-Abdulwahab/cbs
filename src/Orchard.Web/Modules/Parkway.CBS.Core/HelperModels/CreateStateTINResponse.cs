namespace Parkway.CBS.Core.HelperModels
{
    public class CreateStateTINResponse
    {
        public string Name { get; set; }

        public string StateTIN { get; set; }

        public string NormalizedStateTIN { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// Shows if phone number already exist and that the old record for the payer is what was returned
        /// </summary>
        public bool PhoneNumberAlreadyExist { get; set; }
    }
}