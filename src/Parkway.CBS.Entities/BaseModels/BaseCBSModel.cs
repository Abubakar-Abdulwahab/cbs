using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.BaseModels
{
    public abstract class BaseCBSModel
    {
        private DateTime _createdUtc;
        public virtual DateTime CreatedAtUtc
        {
            get
            {
                if (_createdUtc == DateTime.MinValue) { _createdUtc = DateTime.Now.ToLocalTime(); }
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
                if (_updatedAtUtc == null) { _updatedAtUtc = DateTime.Now.ToLocalTime(); }
                return _updatedAtUtc;
            }
            set
            {
                _updatedAtUtc = value;
            }
        }
    }
}
