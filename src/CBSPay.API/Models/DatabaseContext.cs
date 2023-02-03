using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CBSPay.API.App_Start;
using CBSPay.Core.Entities;

namespace CBSPay.API.Models
{
    public class DatabaseContext
    {
        public IUserStore<UserrRecord, int> Users
        {
            get { return new UserStore(); }
        }


        public IRoleStore<Role, int> Roles
        {
            get { return new RoleStore(); }
        }
    }
}