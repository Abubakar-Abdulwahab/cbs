using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Core.Services
{
    public class BVNValidationResponseManager : BaseManager<BVNValidationResponse>, IBVNValidationResponseManager<BVNValidationResponse>
    {
        private readonly IRepository<BVNValidationResponse> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public BVNValidationResponseManager(IRepository<BVNValidationResponse> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Updates tax entity info with most recent validation response from BVN
        /// </summary>
        /// <param name="bvn"></param>
        public void UpdateTaxEntityInfoWithValidationResponseForBVN(string bvn)
        {
            try
            {
                var queryText = $"UPDATE te SET te.Email = (CASE WHEN te.Email != bvnValidationResponse.EmailAddress AND bvnValidationResponse.EmailAddress IS NOT NULL AND " +
                    $"bvnValidationResponse.EmailAddress != '' THEN bvnValidationResponse.EmailAddress ELSE te.Email END), te.Recipient = (CASE WHEN te.Recipient != CONCAT" +
                    $"(bvnValidationResponse.FirstName,' ', bvnValidationResponse.MiddleName, ' ', bvnValidationResponse.LastName) THEN CONCAT(bvnValidationResponse.FirstName,' ', " +
                    $"bvnValidationResponse.MiddleName, ' ', bvnValidationResponse.LastName) ELSE te.Recipient END), te.PhoneNumber = (CASE WHEN te.PhoneNumber != bvnValidationResponse.PhoneNumber " +
                    $"AND bvnValidationResponse.PhoneNumber IS NOT NULL AND bvnValidationResponse.PhoneNumber != '' THEN bvnValidationResponse.PhoneNumber ELSE te.PhoneNumber END), " +
                    $"te.UpdatedAtUtc = GETDATE() FROM Parkway_CBS_Core_BVNValidationResponse as bvnValidationResponse INNER JOIN Parkway_CBS_Core_TaxEntity as te ON " +
                    $"te.IdentificationNumber = bvnValidationResponse.BVN WHERE bvnValidationResponse.Id = (SELECT TOP 1 innerBvnValidationResponse.Id FROM Parkway_CBS_Core_BVNValidationResponse " +
                    $"as innerBvnValidationResponse WHERE innerBvnValidationResponse.BVN = :bvn ORDER BY innerBvnValidationResponse.Id DESC) AND bvnValidationResponse.BVN = :bvn;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("bvn", bvn);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Updates CBS User Info with most recent validation response from BVN
        /// </summary>
        /// <param name="bvn"></param>
        public void UpdateCBSUserInfoWithValidationResponseForBVN(string bvn)
        {
            try
            {
                var queryText = $"UPDATE cb SET cb.Email = (CASE WHEN cb.Email != bvnValidationResponse.EmailAddress AND bvnValidationResponse.EmailAddress IS NOT NULL AND " +
                    $"bvnValidationResponse.EmailAddress != '' THEN bvnValidationResponse.EmailAddress ELSE cb.Email END), cb.Name = (CASE WHEN cb.Name != CONCAT(bvnValidationResponse.FirstName,' ', " +
                    $"bvnValidationResponse.MiddleName, ' ', bvnValidationResponse.LastName) THEN CONCAT(bvnValidationResponse.FirstName,' ', bvnValidationResponse.MiddleName, ' ', " +
                    $"bvnValidationResponse.LastName) ELSE cb.Name END), cb.PhoneNumber = (CASE WHEN cb.PhoneNumber != bvnValidationResponse.PhoneNumber AND bvnValidationResponse.PhoneNumber" +
                    $" IS NOT NULL AND bvnValidationResponse.PhoneNumber != '' THEN bvnValidationResponse.PhoneNumber ELSE cb.PhoneNumber END), cb.UpdatedAtUtc = GETDATE() FROM " +
                    $"Parkway_CBS_Core_BVNValidationResponse as bvnValidationResponse INNER JOIN Parkway_CBS_Core_TaxEntity AS te ON te.IdentificationNumber = bvnValidationResponse.BVN INNER JOIN " +
                    $"Parkway_CBS_Core_CBSUser AS cb ON cb.TaxEntity_Id = te.Id WHERE bvnValidationResponse.Id = (SELECT TOP 1 innerBvnValidationResponse.Id FROM" +
                    $" Parkway_CBS_Core_BVNValidationResponse as innerBvnValidationResponse WHERE innerBvnValidationResponse.BVN = :bvn ORDER BY innerBvnValidationResponse.Id DESC)" +
                    $" AND bvnValidationResponse.BVN = :bvn AND cb.IsAdministrator = :boolval;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("boolval", true);
                query.SetParameter("bvn", bvn);

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