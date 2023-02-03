using System;

namespace Parkway.CBS.Core.Models
{
    public class ActivityPermission : CBSModel
    {
        public virtual CBSPermission CBSPermission { get; set; }

        /// <summary>
        /// Bool value to indicate whether the permission is required or not
        /// </summary>
        public virtual bool Value { get; set; }

        /// <summary>
        /// <see cref="Enums.ActivityType"/>
        /// </summary>
        public virtual int ActivityType { get; set; }

        /// <summary>
        /// This is the Id of the entity we are trying to check the permission for
        /// </summary>
        public virtual Int64 ActivityId { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}