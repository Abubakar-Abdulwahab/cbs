using System;

namespace Parkway.CBS.Core.Models
{
    public abstract class CBSBaseModel
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