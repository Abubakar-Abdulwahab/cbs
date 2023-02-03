using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreCharacterCertificateInputValidation : IDependency
    {

        /// <summary>
        /// Validate the reason for inquiry
        /// </summary>
        /// <param name="userInput">PCCDiasporaUserInputVM</param>
        /// <param name="errors"></param>
        void ValidateReasonForInquiry(CharacterCertificateRequestVM userInput, List<ErrorModel> errors);

        /// <summary>
        /// Validate the place of birth
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidatePlaceOfBirth(CharacterCertificateRequestVM userInput, List<ErrorModel> errors);

        /// <summary>
        /// Validate Date of birth
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidateDateOfBirth(CharacterCertificateRequestVM userInput, List<ErrorModel> errors);


        /// <summary>
        /// Validate country of residence
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidateCountryOfResidence(PCCDiasporaUserInputVM userInput, List<ErrorModel> errors);


        /// <summary>
        /// Validate country of passport
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidateCountryOfPassport(CharacterCertificateRequestVM userInput, List<ErrorModel> errors);

        /// <summary>
        /// Validate passport number
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidatePassportNumber(CharacterCertificateRequestVM userInput, List<ErrorModel> errors);

        /// <summary>
        /// Validate date of passport issurance
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidateDateOfPassportIssurance(CharacterCertificateRequestVM userInput, List<ErrorModel> errors);


        /// <summary>
        /// check file size
        /// </summary>
        /// <param name="uploads"></param>
        /// <param name="errors"></param>
        /// <param name="max file size"></param>
        void CheckFileSize(ICollection<UploadedFileAndName> uploads, List<ErrorModel> errors, int maxFileSize);


        /// <summary>
        /// Check file type
        /// </summary>
        /// <param name="uploads"></param>
        /// <param name="errors"></param>
        void CheckFileType(ICollection<UploadedFileAndName> uploads, List<ErrorModel> errors);


        /// <summary>
        /// Validate NIN details
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValideateNINDetails(PCCDiasporaUserInputVM userInput, List<ErrorModel> errors);

    }
}
