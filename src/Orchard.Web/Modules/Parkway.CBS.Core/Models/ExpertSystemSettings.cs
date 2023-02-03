using Newtonsoft.Json;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.Models
{
    public class ExpertSystemSettings : CBSModel
    {
        /// <summary>
        /// Tenant state record
        /// </summary>
        public virtual TenantCBSSettings TenantCBSSettings { get; set; }

        public virtual string ClientId { get; set; }

        public virtual string ClientSecret { get; set; }

        /// <summary>
        /// TSA bank Id
        /// </summary>
        public virtual int TSA { get; set; }

        /// <summary>
        /// Bank code
        /// </summary>
        public virtual string BankCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(15, ErrorMessage = "Please add a valid bank account number.", MinimumLength = 10)]
        /// <summary>
        /// TSA bank account number
        /// </summary>
        public virtual string TSABankNumber { get; set; }

        [Required(ErrorMessage = "This field is required"), EmailAddress(ErrorMessage = "Please supply a valid email address")]
        [StringLength(250, ErrorMessage = "The email value can only be 250 characters long or 2 characters short", MinimumLength = 2)]
        public virtual string CompanyEmail { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(500, ErrorMessage = "The email value can only be 500 characters long or 2 characters short", MinimumLength = 2)]
        public virtual string CompanyAddress { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [StringLength(255, ErrorMessage = "The company name value can only be 255 characters long or 2 characters short", MinimumLength = 2)]
        public virtual string CompanyName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(15, ErrorMessage = "The company mobile value can only contain 15 or 7 numbers", MinimumLength = 7)]
        public virtual string CompanyMobile { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(500, ErrorMessage = "The business nature value can only be 500 characters long or 2 characters short", MinimumLength = 2)]
        public virtual string BusinessNature { get; set; }

        public virtual string LogoPath { get; set; }

        public virtual string SignaturePath { get; set; }

        /// <summary>
        /// Reference data source type. this is the source type e.g adapter
        /// </summary>
        public virtual string ReferenceDataSourceType { get; set; }

        /// <summary>
        /// Reference data source name, this is the name of the reference data
        /// </summary>
        public virtual string ReferenceDataSourceName { get; set; }

        /// <summary>
        /// if the reference data source type is of adapter type. you are required to provide this field with the class full name got from the config file
        /// </summary>
        public virtual string AdapterClassName { get; set; }

        /// <summary>
        /// LastUpdatedBy id of the admin that added the revenue head record.
        /// Only for audit/reference purposes, always use the LastUpdatedBy property if you want to know
        /// the user that performed the last update, which should suffice for most cases.
        /// </summary>
        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(50, ErrorMessage = "The company name value can only be 50 characters long or 2 characters short", MinimumLength = 2)]
        /// <summary>
        /// Scheduler identifier
        /// </summary>
        public virtual string BillingSchedulerIdentifier { get; set; }


        public virtual bool IsActive { get; set; }

        /// <summary>
        /// This denotes the first expert system all other expert system are derived from
        /// </summary>
        public virtual bool IsRoot { get; set; }

        public virtual string URLPath { get; set; }

        /// <summary>
        /// call back URL for payment notifications
        /// </summary>
        public virtual string CallBackURL { get; set; }

        /// <summary>
        /// json string of revenue head Ids, this expert system can generate invoices for or make requests against
        /// </summary>
        public virtual string AccessList { get; set; }

        /// <summary>
        /// JSON list of permissions
        /// </summary>
        public virtual string PermissionJSONList { get; set; }

        /// <summary>
        /// Payment provider Id <see cref="Models.Enums.PaymentProvider"/>
        /// </summary>
        public virtual int PaymentProviderId { get; set; }


        /// <summary>
        /// this holds the admin that is authorized for admin privileges
        /// </summary>
        public virtual UserPartRecord ThirdPartyAuthorizedAdmin { get; set; }


        /// <summary>
        /// Get the list of permissions for this expert system
        /// </summary>
        /// <returns>List{ExpertSystemPermissionsVM}</returns>
        public List<ExpertSystemPermissionsVM> ListOfPermissions()
        {
            if (string.IsNullOrEmpty(this.PermissionJSONList)) { return new List<ExpertSystemPermissionsVM> { }; }
            return JsonConvert.DeserializeObject<List<ExpertSystemPermissionsVM>>(this.PermissionJSONList);
        }

        /// <summary>
        /// Get the list of revenue head Ids, that this expert system has been tied down to
        /// <para>If not accesslist, an empty list is returned</para>
        /// </summary>
        public List<int> ListOfRevenueHeadAccessList()
        {
            if (string.IsNullOrEmpty(this.AccessList)) { return new List<int> { }; }
            return JsonConvert.DeserializeObject<List<int>>(this.AccessList);
        }

        public string GetThumbnail(string path)
        {
            var segs = path.Split('/');
            var fileName = segs[segs.Length - 1];
            fileName = "Thumbnail/" + fileName.Split(new string[] { ".png" }, StringSplitOptions.None)[0] + "_thumbnail.png";
            segs[segs.Length - 1] = fileName;
            return string.Join("/", segs);
        }
    }
}