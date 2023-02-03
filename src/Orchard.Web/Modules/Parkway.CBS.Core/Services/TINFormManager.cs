using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Orchard;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using NHibernate.Criterion;
using System;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class TINFormManager : BaseManager<TINRegistrationForm>, ITINFormManager<TINRegistrationForm>
    {
        private readonly IRepository<TINRegistrationForm> _repository;
        private readonly IRepository<Applicant> _applicantRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Asset> _assetRepository;
        private readonly IRepository<TaxEntityCategory> _taxEntityCategoryRepository;
        private readonly IRepository<TaxEntity> _taxEntityRepository;

        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreTaxPayerService _coreTaxPayerService;
        private readonly ITransactionManager _transactionManager;

        public TINFormManager(IRepository<TINRegistrationForm> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices,
            IRepository<Applicant> applicantRepository, IRepository<Address> addressRepository, IRepository<Asset> assetRepository,
            IRepository<TaxEntityCategory> taxEntityCategoryRepository, IRepository<TaxEntity> taxEntityRepository, ICoreTaxPayerService coreTaxPayerService) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            _applicantRepository = applicantRepository;
            _addressRepository = addressRepository;
            _assetRepository = assetRepository;
            _taxEntityCategoryRepository = taxEntityCategoryRepository;
            _taxEntityRepository = taxEntityRepository;
            _orchardServices = orchardServices;
            _coreTaxPayerService = coreTaxPayerService;
        }

        public object EndDate { get; private set; }

        public IEnumerable<TINRegistrationForm> GetAllTINApplicants()
        {
            try
            {
                var applicants = _repository.Fetch(x => x.Id > 0);
                //var applicants = _applicantRepository.Fetch(x => x.Id > 0);
                return applicants;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get tax category based on string identifier
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public TaxEntityCategory GetTaxCategory(string individual)
        {
            try
            {
                Logger.Debug("About to get tax category based on string identifier");
                var tax = _taxEntityCategoryRepository.Get(x => x.StringIdentifier == individual);
                return tax;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// save address record
        /// </summary>
        /// <param name="businessAddress"></param>
        public void SaveAddressRecord(Address businessAddress)
        {
            try
            {
                _addressRepository.Create(businessAddress);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// save applicant record to the db
        /// </summary>
        /// <param name="appliantFormData"></param>
        public void SaveApplicantRecord(Applicant appliantFormData)
        {
            try
            {
                _applicantRepository.Create(appliantFormData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// save asset record to the db
        /// </summary>
        /// <param name="assetsRecord"></param>
        public void SaveAssetsRecord(Asset assetsRecord)
        {
            try
            {
                _assetRepository.Create(assetsRecord);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public TaxEntity SaveTaxEntityRecord(TaxEntity taxEnt)
        {
            try
            {
                Logger.Debug("About to save tax entity record");
                //call Ifetayo's endpoint 
                //Yaayyyyyyy!!!!!
                var res = _coreTaxPayerService.ValidateAndSaveTaxEntity(taxEnt, taxEnt.TaxEntityCategory);
                var taxEntity = new TaxEntity();
                //add call to interface
                return res.TaxEntity;
            }
            catch (CannotSaveTaxEntityException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create billing without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("Unknow error, could not save tax entity record");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public void SaveTINRecord(TINRegistrationForm tinRegistrationFormData)
        {
            try
            {
                _repository.Create(tinRegistrationFormData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// query the Applicant Table based on search parameters
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastname"></param>
        /// <param name="phone"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<TINApplicantReportModel> TINApplicantSearch(string firstName, string lastname, string phone, DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                var ReportSession = _transactionManager.GetSession();
                var startDate = Convert.ToDateTime(StartDate).Date;
                var endDate = Convert.ToDateTime(EndDate).Date;

                //var tinCriteria = ReportSession.CreateCriteria<Applicant>("applicant");
                var tinCriteria = ReportSession.CreateCriteria<TINRegistrationForm>("tin");
                tinCriteria.CreateAlias("tin.Applicant", "applicant");

                tinCriteria.Add(Restrictions.Disjunction()
                        .Add(Restrictions.InsensitiveLike("applicant.FirstName", firstName.Trim(), MatchMode.Anywhere))
                        .Add(Restrictions.InsensitiveLike("applicant.LastName", lastname.Trim(), MatchMode.Anywhere))
                        .Add(Restrictions.InsensitiveLike("applicant.Phone", phone.Trim(), MatchMode.Anywhere))
                        .Add(Restrictions.InsensitiveLike("applicant.Phone2", phone.Trim(), MatchMode.Anywhere)));

                if (StartDate == null && EndDate != null)
                {
                    tinCriteria.Add(Restrictions.Le("applicant.DateOfRegistration", endDate));
                }
                if (StartDate != null && EndDate == null)
                {
                    tinCriteria.Add(Restrictions.Ge("applicant.DateOfRegistration", startDate));
                }
                if (StartDate != null && EndDate != null)
                {
                    tinCriteria.Add(Restrictions.Between("applicant.DateOfRegistration", startDate, endDate));
                }
                //tinCriteria.Add(Restrictions.InsensitiveLike("TIN", tin.Trim(), MatchMode.Anywhere));

                var applicantSearch = tinCriteria.List<TINRegistrationForm>();
                //var applicantSearch = tinCriteria.List<Applicant>();


                if (applicantSearch.Count > 0)
                {
                    var tinReport = applicantSearch.Select(tinRecord => new TINApplicantReportModel
                    {
                        TINId = tinRecord.Id,
                        Title = tinRecord.Applicant.Title,
                        FirstName = tinRecord.Applicant.FirstName,
                        LastName = tinRecord.Applicant.LastName,
                        Phone = tinRecord.Applicant.Phone ?? "",
                        Email = tinRecord.Applicant.Email,
                        TINRef = "",
                        DateOfRegistration = Convert.ToDateTime(tinRecord.Applicant.DateOfRegistration),
                        TIN = tinRecord.TIN
                    }).OrderByDescending(w => w.DateOfRegistration).ToList();

                    return tinReport;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                return null;
            }

        }

        /// <summary>
        /// Update TIN Applicant Record
        /// </summary>
        /// <param name="tINId"></param>
        /// <param name="TINValue"></param>
        /// <returns></returns>
        public bool UpdateTINApplicantRecord(long tINId, string TINValue)
        {
            try
            {
                Logger.Debug("About to update TIN Applicant Record");
                var tinRecord = _repository.Get(x => x.Id == tINId);
                if (tinRecord != null)
                {
                    tinRecord.TIN = TINValue;
                    var taxRecord = _taxEntityRepository.Get(x => x.PhoneNumber == tinRecord.TIN && x.Email == tinRecord.Applicant.Email);
                    if (taxRecord != null)
                    {
                        taxRecord.TaxPayerIdentificationNumber = TINValue;
                        _taxEntityRepository.Update(taxRecord);
                        _repository.Update(tinRecord);
                        Logger.Debug("Successfully updated TIN Applicant Record");
                        return true;
                    }
                }
                Logger.Error("Failed to update TIN Applicant Record");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to update TIN Applicant Record");
                Logger.Error(ex.StackTrace, ex);
                return false;
            }
        }
    }
}