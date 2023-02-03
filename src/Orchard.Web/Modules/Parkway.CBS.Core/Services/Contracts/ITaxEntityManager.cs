using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using System;
using Parkway.CBS.FileUpload.BModels;
using System.Linq.Expressions;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxEntityManager<TaxPayer> : IDependency, IBaseManager<TaxPayer>
    {
        /// <summary>
        /// Get the list of tax payers that have been saved along with their cash flow credentials
        /// </summary>
        /// <param name="savedAndUnSavedTaxPayersList"></param>
        /// <returns>IList{TaxPayerModel}</returns>
        IList<CashFlowCustomerTaxPayerModel> CashFlowTaxPayersModelForTaxPayersThatHaveBeenSaved(List<TaxPayer> savedAndUnSavedTaxPayers);

        /// <summary>
        /// Get a joiner between ref data temp and tax enity based on tax identification number
        /// </summary>
        /// <param name="batchNumber">string</param>
        /// <param name="revenueHeadId">int</param>
        /// <param name="billingId">int</param>
        ProcessResponseModel GetRefDataTempJoinerWithTaxIdentification(string batchNumber, int revenueHeadId, int billingId);

        /// <summary>
        /// Get the tax payers for the specified TIN or Phone number
        /// <para>Searches for tax entities with either TIN or Phone number depending on column name</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnName"></param>
        /// <returns>List{TaxPayersWithDetails}</returns>
        List<TaxPayerWithDetails> GetListOfTaxPayersWithDetails(string value, string columnName);


        /// <summary>
        /// Get the identification type for this Id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns>KeyValuePair{int, string}</returns>
        KeyValuePair<int, string> GetIdentificationTypeAndValue(long id);


        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="id"></param>
        /// <returns>TaxPayerWithDetails</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        TaxPayerWithDetails GetTaxPayerWithDetails(Int64 id);


        /// <summary>
        /// Get tax payer category Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>int</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        int GetTaxPayerCtegoryId(Int64 id);


        IEnumerable<TaxEntity> TaxPayerSearch(string name, string category, string RCNumber, string tin, DateTime startDate, DateTime endDate, int take, int skip);

        void SaveFromScrapFile(List<ScrapFileModel> scraps);

        List<TaxPayerWithDetails> GetListOfTaxPayersWithCategoryId(int categoryId);

        /// <summary>
        /// Get tax profile with details
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails GetTaxPayerWithDetails(string payerId);



        bool UpdateTaxPayer(TaxPayerWithDetails model);



        bool CheckIfTaxPayerCodeExist(string taxPayerCode, string taxPayerId);


        /// <summary>
        /// Get tax entity by tax payer code
        /// <para>returns only limited properties</para>
        /// </summary>
        /// <param name="taxPayerCode"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails GetTaxPayerDetailsByTaxPayerCode(string taxPayerCode);

        /// <summary>
        /// Returns Only the tax entity Id
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The Id of the tax entity</returns>
        /// <exception cref="System.Exception">Thrown when no record is found</exception>
        long GetTaxEntityId(Expression<Func<TaxEntity, bool>> lambda);
        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails GetTaxPayerDetailsByPhoneNumber(string phoneNumber);

        /// <summary>
        /// Get tax profile category Id
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns>int</returns>
        int GetTaxPayerCategoryId(string payerId);

        /// <summary>
        /// Gets tax entity details with specified payer id
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        TaxEntityViewModel GetTaxEntityDetailsWithPayerId(string payerId);

        /// <summary>
        /// Get the identity type Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>int</returns>
        int GetIdentityType(long id);
    }
}