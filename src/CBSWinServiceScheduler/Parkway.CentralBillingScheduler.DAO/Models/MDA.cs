using System.Collections.Generic;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public class MDA : CBSModel
    {
        public virtual ICollection<RevenueHead> RevenueHeads { get; set; }

        public virtual string SMEKey { get; set; }

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
    }
}