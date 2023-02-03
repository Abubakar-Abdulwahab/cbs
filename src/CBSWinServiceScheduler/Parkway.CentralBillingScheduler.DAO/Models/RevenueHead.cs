using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public class RevenueHead : CBSModel
    {
        public virtual BillingModel BillingModel { get; set; }

        public virtual MDA Mda { get; set; }

        public virtual string CashFlowProductCode { get; set; }

        public virtual Int64 CashFlowProductId { get; set; }

        public virtual string RefDataURL { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual string Slug { get; set; }


        private bool _active = true;

        public virtual bool IsActive
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
            }
        }
        private bool _visible = false;

        public virtual bool IsVisible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        /// <summary>
        /// Return a concat of a | separated name and code value
        /// </summary>
        /// <returns>string</returns>
        public string NameAndCode()
        {
            return String.Format("{0} | {1}", this.Name, this.Code);
        }
    }
}
