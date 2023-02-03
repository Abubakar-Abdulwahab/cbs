using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public abstract class MDARevenueHead : CBSModel
    {
        [Required(ErrorMessage ="Code field is required")]
        [StringLength(250, ErrorMessage = "The Code value can be 250 characters long or 2 characters short", MinimumLength = 2)]
        public virtual string Code { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        [StringLength(250, ErrorMessage = "The Name value can be 250 characters long or 2 characters short", MinimumLength = 2)]
        public virtual string Name { get; set; }

        public virtual string Slug { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

        /// <summary>
        /// LastUpdatedBy id of the admin that added the revenue head record.
        /// Only for audit/reference purposes, always use the LastUpdatedBy property if you want to know
        /// the user that performed the last update, which should suffice for most cases.
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }
        
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


        public virtual string SettlementCode { get; set; }

        /// <summary>
        /// <see cref="Enums.SettlementType"/>
        /// </summary>
        public virtual int SettlementType { get; set; }

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