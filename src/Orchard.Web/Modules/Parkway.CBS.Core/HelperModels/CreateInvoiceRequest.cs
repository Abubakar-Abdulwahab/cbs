using System;
using System.Collections.Generic;
using System.Text;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateInvoiceRequest : BaseRequestResponse
    {

        /// <summary>
        /// Invoice title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// TaxEntityId of customer to which invoice is sent or issued. Must match the TaxEntityId of the TaxEntity on Cashflow
        /// </summary>
        public Int64 CustomerId { get; set; }

        /// <summary>
        /// Date of invoice creation
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Invoice due date
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Contact TaxEntityId of TaxEntity to which invoice is issued
        /// </summary>
        public Int64 ContactID { get; set; }

        /// <summary>
        /// The type of Discount which should be applied to the issued invoice
        /// Leave as "None" for No Discount, "Flat" for a flat amount , "Percent" for a percentage discount
        /// </summary>
        public String DiscountType { get; set; }

        /// <summary>
        /// Type of invoice "Single" , "Group", "Individual"
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// Discount Value [Optional] Leave blank if "DiscountType" is None.                
        /// </summary>
        public Decimal Discount { get; set; }

        /// <summary>
        /// TaxEntityId of reminder that should be attached to the invoice.
        /// </summary>
        public Int64 ReminderId { get; set; }

        /// <summary>
        /// A list of Products/ InvoiceItems that should be included in the invoice
        /// </summary>
        public List<ProductModel> Items { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var properties = this.GetType().GetProperties();

            var members = this.GetType().GetMembers();

            foreach (var property in properties)
            {
                if (property.MemberType == System.Reflection.MemberTypes.NestedType)
                {
                    var mems = property.GetType().GetProperties();
                    foreach (var _mem in mems)
                    {
                        sb.AppendLine(string.Format("Name: {0}  Value: {1}", _mem.Name, _mem.GetValue(_mem)));

                    }
                }
                else
                {
                    sb.AppendLine(string.Format("Name: {0}  Value: {1}", property.Name, property.GetValue(this)));

                }
            }

            return sb.ToString();
        }
    }
}