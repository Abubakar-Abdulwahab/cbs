using Orchard;
using Orchard.Logging;
using Orchard.Security;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Parkway.CBS.Core.NationalIdentityVerifcationSystem;
using System.Numerics;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreNINValidationService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        public ILogger Logger { get; set; }
        public CoreNINValidationService(IOrchardServices orchardServices, IMembershipService membershipService)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Validate National Identification Number
        /// </summary>
        /// <param name="nin"></param>
        /// <param name="errormessage"></param>
        /// <returns></returns>
        public dynamic ValidateNIN(string nin, out string errormessage)
        {
            errormessage = string.Empty;
            try
            {

                string username = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.NINValidationUsername)];
                string password = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.NINValidationPassword)];
                string orgid = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.NINValidationOrgid)];

                #region NIVS Service Impl
                //create instance of NIVS proxy class
                IdentitySearchClient identitySearchClient = new IdentitySearchClient();
                //call createToken method to fetch one time token object from which its loginString property would be used to make subsequent function calls
                //hash and encrypt password
                tokenObject tokenObj = identitySearchClient.createToken(username, HashAndEncryptPassword(password), orgid);
                if (string.IsNullOrEmpty(tokenObj.loginString)) { throw new Exception("Unable to get NIN token string"); }
                //call searchByNIN
                searchResponseDemo searchByNINResponse = identitySearchClient.searchByNIN(tokenObj.loginString, nin);
                #endregion

                if (searchByNINResponse == null || searchByNINResponse.data == null || searchByNINResponse.data.Count() == 0)
                {
                    return null;
                }

                return new NINValidationImplResponseModel { 
                    BatchId = searchByNINResponse.data.First().batchid,
                    BirthCountry = searchByNINResponse.data.First().birthcountry,
                    BirthDate = searchByNINResponse.data.First().birthdate,
                    BirthLga = searchByNINResponse.data.First().birthlga,
                    BirthState = searchByNINResponse.data.First().birthstate,
                    CardStatus = searchByNINResponse.data.First().cardstatus,
                    CentralID = searchByNINResponse.data.First().centralID,
                    DocumentNo = searchByNINResponse.data.First().documentno,
                    EducationalLevel = searchByNINResponse.data.First().educationallevel,
                    Email = searchByNINResponse.data.First().email,
                    EmploymentStatus = searchByNINResponse.data.First().emplymentstatus,
                    FirstName = searchByNINResponse.data.First().firstname,
                    Gender = searchByNINResponse.data.First().gender,
                    Height = searchByNINResponse.data.First().heigth,
                    MaritalStatus = searchByNINResponse.data.First().maritalstatus,
                    MiddleName = searchByNINResponse.data.First().middlename,
                    NIN = searchByNINResponse.data.First().nin,
                    NextOfKinAddress1 = searchByNINResponse.data.First().nok_address1,
                    NextOfKinAddress2 = searchByNINResponse.data.First().nok_address2,
                    NextOfKinFirstName = searchByNINResponse.data.First().nok_firstname,
                    NextOfKinLGA = searchByNINResponse.data.First().nok_lga,
                    NextOfKinMiddleName = searchByNINResponse.data.First().nok_middlename,
                    NextOfKinState = searchByNINResponse.data.First().nok_state,
                    NextOfKinSurname = searchByNINResponse.data.First().nok_surname,
                    NextOfKinTown = searchByNINResponse.data.First().nok_town,
                    NativeSpokenLang = searchByNINResponse.data.First().nspokenlang,
                    Photo = searchByNINResponse.data.First().photo,
                    Profession = searchByNINResponse.data.First().profession,
                    Religion = searchByNINResponse.data.First().religion,
                    ResidenceAdressLine1 = searchByNINResponse.data.First().residence_AdressLine1,
                    ResidenceTown = searchByNINResponse.data.First().residence_Town,
                    ResidenceLGA = searchByNINResponse.data.First().residence_lga,
                    ResidenceState = searchByNINResponse.data.First().residence_state,
                    ResidenceStatus = searchByNINResponse.data.First().residencestatus,
                    SelfOriginLGA = searchByNINResponse.data.First().self_origin_lga,
                    SelfOriginPlace = searchByNINResponse.data.First().self_origin_place,
                    SelfOriginState = searchByNINResponse.data.First().self_origin_state,
                    Signature = searchByNINResponse.data.First().signature,
                    Surname = searchByNINResponse.data.First().surname,
                    TelephoneNo = searchByNINResponse.data.First().telephoneno,
                    Title = searchByNINResponse.data.First().title,
                    TrackingId = searchByNINResponse.data.First().trackingId
                };
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (AggregateException)
            {
                throw;
            }
            catch (Exception exception) { Logger.Error(exception, $"Unable to validate NIN {nin}. Exception message --- {exception.Message}"); throw; }
        }


        /// <summary>
        /// Performs SHA256 hash and RSA encryption of password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string HashAndEncryptPassword(string password)
        {
            byte[] encryptedPassword;

            string exponent = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.NINValidationExponent)];

            string modulus = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.NINValidationModulus)];

            BigInteger e = BigInteger.Parse(exponent);
            BigInteger m = BigInteger.Parse(modulus);
            string pwd = GetPasswordHash(password);
            BigInteger pm = new BigInteger(ToLittleEndian(pwd));
            BigInteger b = BigInteger.ModPow(pm, e, m);
            encryptedPassword = Encoding.UTF8.GetBytes(b.ToString());

            return Convert.ToBase64String(encryptedPassword);
        }


        /// <summary>
        /// Computes password hash according to National Identity Verification System requirements
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private static string GetPasswordHash(string pwd)
        {
            HashAlgorithm m = SHA256.Create();
            byte[] outputBytes = m.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            return BitConverter.ToString(outputBytes).Replace("-", "").ToLower();
        }


        /// <summary>
        /// Reverses the order of the bigEndianHex bytes, changing it to little bytes to support .net BigInteger Constructor without BigEndian parameter
        /// </summary>
        /// <param name="bigEndianHex"></param>
        /// <returns></returns>
        private static byte[] ToLittleEndian(string bigEndianHex)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(bigEndianHex);
            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().Concat(new byte[] { 0 }).ToArray();
            }

            return bytes;
        }

    }
}