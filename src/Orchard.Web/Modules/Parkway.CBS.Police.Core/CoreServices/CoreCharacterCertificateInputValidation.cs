using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreCharacterCertificateInputValidation : ICoreCharacterCertificateInputValidation
    {

        private readonly ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry> _requestInquiryRepo;
        private readonly ICoreCountryService _coreCountryService;
        private readonly ICoreHelperService _corehelper;
        private readonly ITaxEntityManager<TaxEntity> _taxPayerRepo;
        private readonly IIdentificationTypeManager<IdentificationType> _identificationTypeRepo;
        private readonly IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> _identityTypeTaxCategoryRepo;
        private readonly IEnumerable<IIdentificationNumberValidationImpl> _identificationNumbervalidationImpl;
        public ILogger Logger { get; set; }

        public CoreCharacterCertificateInputValidation(ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry> requestInquiryRepo, ICoreCountryService coreCountryService, ICoreHelperService corehelper, ITaxEntityManager<TaxEntity> taxPayerRepo, IIdentificationTypeManager<IdentificationType> identificationTypeRepo, IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> identityTypeTaxCategoryRepo, IEnumerable<IIdentificationNumberValidationImpl> identificationNumbervalidationImpl)
        {
            _requestInquiryRepo = requestInquiryRepo;
            _coreCountryService = coreCountryService;
            _corehelper = corehelper;
            _taxPayerRepo = taxPayerRepo;
            _identificationTypeRepo = identificationTypeRepo;
            _identityTypeTaxCategoryRepo = identityTypeTaxCategoryRepo;
            _identificationNumbervalidationImpl = identificationNumbervalidationImpl;
            Logger = NullLogger.Instance;
        }       


        /// <summary>
        /// Check file type
        /// </summary>
        /// <param name="uploads"></param>
        /// <param name="errors"></param>
        public void CheckFileType(ICollection<UploadedFileAndName> uploads, List<ErrorModel> errors)
        {
            foreach (var item in uploads)
            {
                _corehelper.CheckFileType(new List<UploadedFileAndName>(1) { item }, errors, item.AcceptedMimes, item.AcceptedExtensions);
            }
        }


        /// <summary>
        /// check file size
        /// </summary>
        /// <param name="uploads"></param>
        /// <param name="errors"></param>
        /// <param name="max file size"></param>
        public void CheckFileSize(ICollection<UploadedFileAndName> uploads, List<ErrorModel> errors, int maxFileSize)
        {
            _corehelper.CheckFileSize(uploads.ToList(), errors, maxFileSize);
        }



        /// <summary>
        /// Validate date of passport issurance
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValidateDateOfPassportIssurance(CharacterCertificateRequestVM userInput, List<ErrorModel> errors)
        {
            if (string.IsNullOrEmpty(userInput.DateOfIssuance))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Date of issuance is required", FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance) });
                return;
            }

            try
            {
                userInput.DateOfIssuanceParsed = DateTime.ParseExact(userInput.DateOfIssuance.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (userInput.DateOfIssuanceParsed > DateTime.Now)
                {
                    errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance), ErrorMessage = "Please input a valid date. Date of issuance cannot be a date in the future." });
                }
            }
            catch (Exception)
            {
                errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance), ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
            }
        }


        /// <summary>
        /// Validate passport number
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValidatePassportNumber(CharacterCertificateRequestVM userInput, List<ErrorModel> errors)
        {
            if (string.IsNullOrEmpty(userInput.PassportNumber))
            {
                errors.Add(new ErrorModel { ErrorMessage = "International passport number is required", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
                return;
            }

            if (_coreCountryService.checkIfCountryIsNigeria(userInput.SelectedCountryOfPassport))
            {
                if (userInput.PassportNumber.Trim().Length != 9)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "International passport number must be 9 characters(a letter and 8 digits)", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
                }
                return;
            }

            if (userInput.PassportNumber.Trim().Length < 7 || userInput.PassportNumber.Trim().Length > 9)
            {
                errors.Add(new ErrorModel { ErrorMessage = "International passport number must be at least 7 characters and 9 characters at most", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
            }
        }


        /// <summary>
        /// Validate country of passport
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValidateCountryOfPassport(CharacterCertificateRequestVM userInput, List<ErrorModel> errors)
        {
            if (userInput.SelectedCountryOfPassport > 0)
            {
                if (!_coreCountryService.ValidateCountry(userInput.SelectedCountryOfPassport))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected country of passport is not valid", FieldName = nameof(CharacterCertificateRequestVM.SelectedCountryOfPassport) });
                }
            }
        }


        /// <summary>
        /// Validate country of residence
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValidateCountryOfResidence(PCCDiasporaUserInputVM userInput, List<ErrorModel> errors)
        {
            if (userInput.SelectedCountryOfPassport > 0)
            {
                if (!_coreCountryService.ValidateCountry(userInput.SelectedCountryOfResidence))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected country of residence is not valid", FieldName = nameof(PCCDiasporaUserInputVM.SelectedCountryOfResidence) });
                }
            }
        }


        /// <summary>
        /// Validate Date of birth
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValidateDateOfBirth(CharacterCertificateRequestVM userInput, List<ErrorModel> errors)
        {
            if (string.IsNullOrEmpty(userInput.DateOfBirth))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Date of birth is required", FieldName = nameof(CharacterCertificateRequestVM.DateOfBirth) });
            }

            try
            {
                userInput.DateOfBirthParsed = DateTime.ParseExact(userInput.DateOfBirth.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (userInput.DateOfBirthParsed > DateTime.Now.AddYears(-14))
                {
                    errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfBirth), ErrorMessage = $"Please input a valid date. Date of birth cannot be more than {DateTime.Now.AddYears(-14).ToString("dd/MM/yyyy")}." });
                }
            }
            catch (Exception)
            {
                errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfBirth), ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
            }
        }


        /// <summary>
        /// Validate the place of birth
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValidatePlaceOfBirth(CharacterCertificateRequestVM userInput, List<ErrorModel> errors)
        {
            if (string.IsNullOrEmpty(userInput.PlaceOfBirth))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Place of birth is required. Must be between 3 - 50 characters", FieldName = nameof(CharacterCertificateRequestVM.PlaceOfBirth) });
                return;
            }

            userInput.PlaceOfBirth = userInput.PlaceOfBirth.Trim();
            if (userInput.PlaceOfBirth.Length < 3 || userInput.PlaceOfBirth.Length > 50)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Place of birth is required. Must be between 3 - 50 characters", FieldName = nameof(CharacterCertificateRequestVM.PlaceOfBirth) });
            }
        }


        /// <summary>
        /// Validate the reason for inquiry
        /// </summary>
        /// <param name="userInput">PCCDiasporaUserInputVM</param>
        /// <param name="errors"></param>
        public void ValidateReasonForInquiry(CharacterCertificateRequestVM userInput, List<ErrorModel> errors)
        {
            CharacterCertificateReasonForInquiryVM reasonForInquiry = _requestInquiryRepo.GetReasonForInquiry(userInput.CharacterCertificateReasonForInquiry).SingleOrDefault();

            if (reasonForInquiry == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Selected reason for inquiry is not valid", FieldName = nameof(CharacterCertificateRequestVM.CharacterCertificateReasonForInquiry) });
                return;
            }

            if (reasonForInquiry.ShowFreeForm)
            {
                if (!string.IsNullOrEmpty(userInput.ReasonForInquiryValue))
                {
                    userInput.ReasonForInquiryValue = userInput.ReasonForInquiryValue.Trim();
                    if (userInput.ReasonForInquiryValue.Length < 5 || userInput.ReasonForInquiryValue.Length > 20)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Reason for inquiry is required. Must be between 5 - 20 characters", FieldName = nameof(CharacterCertificateRequestVM.ReasonForInquiryValue) });
                        userInput.ShowReasonForInquiryFreeForm = true;
                    }
                }
                else
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Reason for inquiry is required. Must be between 5 - 20 characters", FieldName = nameof(CharacterCertificateRequestVM.ReasonForInquiryValue) });
                    userInput.ShowReasonForInquiryFreeForm = true;
                }
            }
            else { userInput.ReasonForInquiryValue = reasonForInquiry.Name; }
        }

        public void ValideateNINDetails(PCCDiasporaUserInputVM userInput, List<ErrorModel> errors)
        {
            throw new NotImplementedException();
        }
    }
}