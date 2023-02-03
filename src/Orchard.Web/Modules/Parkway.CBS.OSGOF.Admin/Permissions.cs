using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission List = new Permission { Description = "View Cell Sites list", Name = "CellSitesList" };
        public static readonly Permission AddOperator = new Permission { Description = "Add new operator", Name = "AddOperator" };
        public static readonly Permission AddCellSites = new Permission { Description = "Add new cell sites", Name = "AddCellSites" };


        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype() {
                    Name = "Administrator",
                    Permissions = new[] { List, AddOperator, AddCellSites, }
                },
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
{
                List,
                AddOperator,
                AddCellSites,
            };
        }
    }
}