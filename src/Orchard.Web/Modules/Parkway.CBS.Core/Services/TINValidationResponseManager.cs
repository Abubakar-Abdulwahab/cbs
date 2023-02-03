using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Orchard.Logging;
using System;

namespace Parkway.CBS.Core.Services
{
    public class TINValidationResponseManager : BaseManager<TINValidationResponse>, ITINValidationResponseManager<TINValidationResponse>
    {
        private readonly IRepository<TINValidationResponse> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TINValidationResponseManager(IRepository<TINValidationResponse> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Updates tax entity info with most recent validation response from TIN
        /// </summary>
        /// <param name="tin"></param>
        public void UpdateTaxEntityInfoWithValidationResponseForTIN(string tin)
        {
            try
            {
                var queryText = $"UPDATE te SET te.Email = (CASE WHEN te.Email != tinValidationResponse.Email AND tinValidationResponse.Email IS NOT NULL AND tinValidationResponse.Email != '' " +
                    $"THEN tinValidationResponse.Email ELSE te.Email END), te.Recipient = (CASE WHEN te.Recipient != tinValidationResponse.TaxPayerName THEN tinValidationResponse.TaxPayerName ELSE" +
                    $" te.Recipient END), te.PhoneNumber = (CASE WHEN te.PhoneNumber != tinValidationResponse.Phone AND tinValidationResponse.Phone IS NOT NULL AND tinValidationResponse.Phone != '' " +
                    $"THEN tinValidationResponse.Phone ELSE te.PhoneNumber END), te.UpdatedAtUtc = GETDATE() FROM Parkway_CBS_Core_TINValidationResponse as tinValidationResponse INNER JOIN " +
                    $"Parkway_CBS_Core_TaxEntity as te ON te.IdentificationNumber = tinValidationResponse.TIN WHERE tinValidationResponse.Id = (SELECT TOP 1 innerTinValidationResponse.Id FROM " +
                    $"Parkway_CBS_Core_TINValidationResponse as innerTinValidationResponse WHERE innerTinValidationResponse.TIN = :tin ORDER BY innerTinValidationResponse.Id DESC) AND " +
                    $"tinValidationResponse.TIN = :tin;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("tin", tin);

                query.ExecuteUpdate();

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Updates CBS User Info with most recent validation response from TIN
        /// </summary>
        /// <param name="tin"></param>
        public void UpdateCBSUserInfoWithValidationResponseForTIN(string tin)
        {
            try
            {
                var queryText = $"UPDATE cb SET cb.Email = (CASE WHEN cb.Email != tinValidationResponse.Email AND tinValidationResponse.Email IS NOT NULL AND tinValidationResponse.Email != '' THEN" +
                    $" tinValidationResponse.Email ELSE cb.Email END), cb.Name = (CASE WHEN cb.Name != tinValidationResponse.TaxPayerName THEN tinValidationResponse.TaxPayerName ELSE cb.Name END), " +
                    $"cb.PhoneNumber = (CASE WHEN cb.PhoneNumber != tinValidationResponse.Phone AND tinValidationResponse.Phone IS NOT NULL AND tinValidationResponse.Phone != '' THEN " +
                    $"tinValidationResponse.Phone ELSE cb.PhoneNumber END), cb.UpdatedAtUtc = GETDATE() FROM Parkway_CBS_Core_TINValidationResponse as tinValidationResponse INNER JOIN " +
                    $"Parkway_CBS_Core_TaxEntity AS te ON te.IdentificationNumber = tinValidationResponse.TIN INNER JOIN Parkway_CBS_Core_CBSUser AS cb ON cb.TaxEntity_Id = te.Id WHERE " +
                    $"tinValidationResponse.Id = (SELECT TOP 1 innerTinValidationResponse.Id FROM Parkway_CBS_Core_TINValidationResponse as innerTinValidationResponse WHERE " +
                    $"innerTinValidationResponse.TIN = :tin ORDER BY innerTinValidationResponse.Id DESC) AND tinValidationResponse.TIN = :tin AND cb.IsAdministrator = :boolval;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("boolval", true);
                query.SetParameter("tin", tin);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}