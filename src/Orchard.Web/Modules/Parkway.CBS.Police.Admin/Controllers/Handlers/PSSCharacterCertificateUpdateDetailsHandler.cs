using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Orchard.Logging;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Orchard;
using Parkway.CBS.Core.Exceptions;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System.Globalization;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSCharacterCertificateUpdateDetailsHandler : IPSSCharacterCertificateUpdateDetailsHandler
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> _repo;
        private readonly ICoreCountryService _coreCountryService;
        ILogger Logger { get; set; }

        public PSSCharacterCertificateUpdateDetailsHandler(IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> repo, IOrchardServices orchardServices, ICoreCountryService coreCountryService)
        {
            _repo = repo;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _coreCountryService = coreCountryService;
        }


        /// <summary>
        /// Get PCC details using filenumber
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>CharacterCertificateDetailsUpdateVM</returns>
        public CharacterCertificateDetailsUpdateVM GetFileNumberDetails(string fileNumber)
        {
            try
            {
                CharacterCertificateDetailsUpdateVM result = _repo.GetCharacterCertificateDetailsForEdit(fileNumber);
                if (result == null) { throw new NoRecordFoundException($"No record found for file number {fileNumber} pending approval"); }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all active countries
        /// </summary>
        /// <returns>IEnumerable<CountryVM></returns>
        public IEnumerable<CountryVM> GetCountries()
        {
            return _coreCountryService.GetCountries();
        }

        /// <summary>
        /// Update character certificate details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        public bool UpdateCharacterCertificateDetails(CharacterCertificateDetailsUpdateVM model, out List<ErrorModel> errors)
        {
            errors = new List<ErrorModel>();
            try
            {
                if (!string.IsNullOrEmpty(model.DateOfIssuanceString))
                {
                    try
                    {
                        model.DateOfIssuance = DateTime.ParseExact(model.DateOfIssuanceString.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (model.DateOfIssuance > DateTime.Now)
                        {
                            errors.Add(new ErrorModel { FieldName = nameof(model.DateOfIssuance), ErrorMessage = "Please input a valid date. Date of issuance cannot be a date in the future." });
                        }
                    }
                    catch (Exception)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(model.DateOfIssuance), ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
                    }
                }
                else { errors.Add(new ErrorModel { ErrorMessage = "Date of issuance is required", FieldName = nameof(model.DateOfIssuance) }); }

                if (!string.IsNullOrEmpty(model.PassportNumber))
                {
                    if (_coreCountryService.checkIfCountryIsNigeria(model.CountryOfPassportId))
                    {
                        if (model.PassportNumber.Trim().Length != 9)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "International passport number must be 9 characters(a letter and 8 digits)", FieldName = nameof(model.PassportNumber) });
                        }
                    }
                    else if (model.PassportNumber.Trim().Length < 7 || model.PassportNumber.Trim().Length > 9)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "International passport number must be at least 7 characters and 9 characters at most", FieldName = nameof(model.PassportNumber) });
                    }
                }

                if (!_coreCountryService.ValidateCountry(model.DestinationCountryId))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected destination country is not valid", FieldName = nameof(model.DestinationCountryId) });
                }
                else
                {
                    model.DestinationCountry = _coreCountryService.GetCountryName(model.DestinationCountryId);
                }

                if (string.IsNullOrEmpty(model.PlaceOfIssuance))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Place of issuance is required", FieldName = nameof(model.PlaceOfIssuance) });
                }
                else
                {
                    if (model.PlaceOfIssuance.Trim().Length < 3 || model.PlaceOfIssuance.Trim().Length > 50)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Place of issuance is required. Must be between 3 - 50 characters", FieldName = nameof(model.PlaceOfIssuance) });
                    }
                }

                if (errors.Count > 0) { throw new DirtyFormDataException { }; }

                return _repo.UpdateCharacterCertificateDetails(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}