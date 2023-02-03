using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class ProductModel
    {
        /// <summary>
        /// Product TaxEntityId
        /// </summary>
        public Int64 ProductId { get; set; }

        /// <summary>
        /// Quantity of product in the invoice
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }
        public int Pos { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }
    }
}