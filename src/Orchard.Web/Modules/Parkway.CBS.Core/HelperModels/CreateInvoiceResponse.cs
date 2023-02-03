using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateInvoiceResponse : BaseRequestResponse
    {
        public Int64 Id { get; set; }
        public virtual string Title { get; set; }
        public Int64 CustomerId { get; set; }
        public IList<ProductModel> Items { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public Int64 ContactID { get; set; }
        public string DiscountType { get { return "None"; } }
        public string Type { get { return "Single"; } }
        public string Number { get; set; }
        public decimal AmountDue { get; set; }
        public string IntegrationPreviewUrl { get; set; }

        public string PreviewUrl { get; set; }
        public string PdfUrl { get; set; }
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