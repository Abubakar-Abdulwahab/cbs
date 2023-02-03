using Orchard;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Orchard.Environment.ShellBuilders;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreTaxPayerService : IDependency
    {
        /// <summary>
        /// Validate and save tax profile
        /// <para>Saves the tax entity details, and account</para>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="category"></param>
        /// <returns>TaxEntityProfileHelper</returns>
        /// <exception cref="CannotSaveTaxEntityException"></exception>
        TaxEntityProfileHelper ValidateAndSaveTaxEntity(TaxEntity entity, TaxEntityCategory category);

        /// <summary>
        /// Check that this category is valid
        /// </summary>
        /// <returns>bool</returns>
        bool CategoryExists(int categoryId);

        /// <summary>
        /// Generate payerID for this Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string</returns>
        string GetPayerId(long id);


        /// <summary>
        /// Get the identity type Id for this tax entity Id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        int GetIdentityType(long id);


        /// <summary>
        /// Get distinct ref data records and thier duplicates if any
        /// </summary>
        /// <param name="entitesWithoutCashflowRecord"></param>
        /// <returns>ProcessResponseModel | if no errors <see cref="RefDataDistinctGroupModel"/>RefDataDistinctGroupModel</returns>
        ProcessResponseModel GetDistinctRefDataRecordsItemsAndDuplicates(ConcurrentStack<RefDataAndCashflowDetails> entitesWithoutCashflowRecord);

        /// <summary>
        /// Get joiner of tax entity table with cash flow details and ref data temp for the given batch number, revenue head and billing id
        /// </summary>
        /// <param name="shellContext"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        /// <returns></returns>
        ProcessResponseModel GetCashflowCredentialsAlongWithRefDatadetails(ShellContext shellContext, string batchNumber, int revenueHeadId, int billingId);

        TaxEntity GetTaxEntity(Expression<Func<TaxEntity, bool>> lambda);

        /// <summary>
        /// Get entity by tax payer code
        /// <para>returns limited object props</para>
        /// </summary>
        /// <param name="taxPayerCode"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails GetTaxEntityDetails(string taxPayerCode);

        /// <summary>
        /// Returns Only the tax entity Id
        /// </summary>
        /// <param name="lambda">Query</param>
        /// <returns>The Id of the tax entity</returns>
        /// <exception cref="NoRecordFoundException"> Thrown when no record is found matching the query</exception>
        long GetTaxEntityId(Expression<Func<TaxEntity, bool>> lambda);

        /// <summary>
        /// Get tax entities that belong to this category, with tcategory Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>List{TaxPayerWithDetails}</returns>
        List<TaxPayerWithDetails> GetTaxEntitiesByCategory(int categoryId);

        //IEnumerable<TaxEntity> GetTaxEntities(Expression<Func<TaxEntity, bool>> lambda);

        int CheckCountCount(Expression<Func<TaxEntity, bool>> lambda);
    }
}