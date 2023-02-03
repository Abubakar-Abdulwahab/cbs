using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public abstract class CBSBaseModel
    {
        private DateTime _createdUtc;
        public virtual DateTime CreatedAtUtc
        {
            get
            {
                if (_createdUtc == DateTime.MinValue) { _createdUtc = DateTime.UtcNow; }
                return _createdUtc;
            }
            private set
            {
                if (_createdUtc == DateTime.MinValue) { _createdUtc = value; }
            }
        }

        private DateTime? _updatedAtUtc;
        public virtual DateTime? UpdatedAtUtc
        {
            get
            {
                if (_updatedAtUtc == null) { _updatedAtUtc = DateTime.UtcNow; }
                return _updatedAtUtc;
            }
            set
            {
                _updatedAtUtc = value;
            }
        }
    }
}
