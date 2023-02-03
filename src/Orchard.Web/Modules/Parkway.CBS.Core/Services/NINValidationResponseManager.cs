using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class NINValidationResponseManager : BaseManager<NINValidationResponse>, ININValidationResponseManager<NINValidationResponse>
    {
        private readonly IRepository<NINValidationResponse> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public NINValidationResponseManager(IRepository<NINValidationResponse> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Updates tax entity info with most recent validation response from NIN
        /// </summary>
        /// <param name="nin"></param>
        public void UpdateTaxEntityInfoWithValidationResponseForNIN(string nin)
        {
            try
            {
                var queryText = $"UPDATE te SET te.Email = (CASE WHEN te.Email != ninValidationResponse.Email AND ninValidationResponse.Email IS NOT NULL AND ninValidationResponse.Email != '' " +
                    $"THEN ninValidationResponse.Email ELSE te.Email END), te.Recipient =(CASE WHEN te.Recipient != CONCAT(ninValidationResponse.FirstName,' ', ninValidationResponse.MiddleName, ' '," +
                    $" ninValidationResponse.Surname) THEN CONCAT(ninValidationResponse.FirstName,' ', ninValidationResponse.MiddleName, ' ', ninValidationResponse.Surname) ELSE te.Recipient END)," +
                    $" te.PhoneNumber = (CASE WHEN te.PhoneNumber != ninValidationResponse.TelephoneNo AND ninValidationResponse.TelephoneNo IS NOT NULL AND ninValidationResponse.TelephoneNo != '' " +
                    $"THEN ninValidationResponse.TelephoneNo ELSE te.PhoneNumber END), te.UpdatedAtUtc = GETDATE() FROM Parkway_CBS_Core_NINValidationResponse as ninValidationResponse INNER JOIN " +
                    $"Parkway_CBS_Core_TaxEntity as te ON te.IdentificationNumber = ninValidationResponse.NIN WHERE ninValidationResponse.Id = (SELECT TOP 1 innerNinValidationResponse.Id FROM " +
                    $"Parkway_CBS_Core_NINValidationResponse as innerNinValidationResponse WHERE innerNinValidationResponse.NIN = te.IdentificationNumber ORDER BY innerNinValidationResponse.Id DESC) " +
                    $"AND ninValidationResponse.NIN = :nin;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("nin", nin);

                query.ExecuteUpdate();

            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Updates CBS User Info with most recent validation response from NIN
        /// </summary>
        /// <param name="nin"></param>
        public void UpdateCBSUserInfoWithValidationResponseForNIN(string nin)
        {
            try
            {
                var queryText = $"UPDATE cb SET cb.Email = (CASE WHEN cb.Email != ninValidationResponse.Email AND ninValidationResponse.Email IS NOT NULL AND ninValidationResponse.Email != '' " +
                    $"THEN ninValidationResponse.Email ELSE cb.Email END), cb.Name = (CASE WHEN cb.Name != CONCAT(ninValidationResponse.FirstName,' ', ninValidationResponse.MiddleName, ' ', " +
                    $"ninValidationResponse.Surname) THEN CONCAT(ninValidationResponse.FirstName,' ', ninValidationResponse.MiddleName, ' ', ninValidationResponse.Surname) ELSE cb.Name END), " +
                    $"cb.PhoneNumber = (CASE WHEN cb.PhoneNumber != ninValidationResponse.TelephoneNo AND ninValidationResponse.TelephoneNo IS NOT NULL AND ninValidationResponse.TelephoneNo != '' " +
                    $"THEN ninValidationResponse.TelephoneNo ELSE cb.PhoneNumber END), cb.UpdatedAtUtc = GETDATE() FROM Parkway_CBS_Core_NINValidationResponse as ninValidationResponse INNER JOIN " +
                    $"Parkway_CBS_Core_TaxEntity AS te ON te.IdentificationNumber = ninValidationResponse.NIN INNER JOIN Parkway_CBS_Core_CBSUser AS cb ON cb.TaxEntity_Id = te.Id WHERE " +
                    $"ninValidationResponse.Id = (SELECT TOP 1 innerNinValidationResponse.Id FROM Parkway_CBS_Core_NINValidationResponse as innerNinValidationResponse WHERE " +
                    $"innerNinValidationResponse.NIN = :nin ORDER BY innerNinValidationResponse.Id DESC) AND ninValidationResponse.NIN = :nin AND cb.IsAdministrator = :boolval;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("boolval", true);
                query.SetParameter("nin", nin);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the last nin validation response with specified nin
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        public LatestNINValidationResponseVM GetNINValidationResponse(string nin)
        {
            try
            {
                return _transactionManager.GetSession().Query<NINValidationResponse>()
                    .Where(r => r.NIN == nin).OrderByDescending(r => r.CreatedAtUtc).Select(r => new LatestNINValidationResponseVM { CreatedAtUtc = r.CreatedAtUtc, BirthDate = r.BirthDate}).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}