using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ETCC.Admin
{
    public class Permissions : IPermissionProvider
    {
        //if you can create you can view
        //if u have the create permission you can view, if u only have the view permission u cannot create
        public static readonly Permission CanViewTCCRequests = new Permission { Description = "Can view TCC request", Name = "CanViewTCCRequests" };
        public static readonly Permission CanApproveTCCRequests = new Permission { Description = "Can approve TCC request", Name = "CanApproveTCCRequests" };
        public static readonly Permission CanViewReceiptUtilizations = new Permission { Description = "Can view receipt utilizations report", Name = "CanViewReceiptUtilizations" };
        public static readonly Permission ViewDirectAssessmentReport = new Permission { Description = "View Direct Assessment Report", Name = "ViewDirectAssessmentReport" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype() {Name = "Administrator", Permissions = new[]
                { CanViewTCCRequests, CanApproveTCCRequests, CanViewReceiptUtilizations} },
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                CanViewTCCRequests,
                CanApproveTCCRequests,
                CanViewReceiptUtilizations,
                ViewDirectAssessmentReport
            };
        }
    }
}