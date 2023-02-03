using Parkway.CBS.TaxPayerEnumerationService.Models;
using System.Collections.Generic;

namespace Parkway.CBS.TaxPayerEnumerationService.Implementations.Contracts
{
    public interface ITaxPayerEnumerationValidation
    {
        /// <summary>
        /// Returns validated and unique enumeration items.
        /// </summary>
        /// <param name="lineItems">List<TaxPayerEnumerationLine></param>
        /// <returns></returns>
        dynamic GetValidatedEnumerationItems(dynamic lineItems);

        /// <summary>
        /// Validates enumeration line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        TaxPayerEnumerationLine ValidateEnumerationLineItem(List<string> lineValues);

        /// <summary>
        /// Ensures there are no duplicate entries in the enumertion collection.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        List<TaxPayerEnumerationLine> GetUniqueEnumerationItems(List<TaxPayerEnumerationLine> items);
    }
}
