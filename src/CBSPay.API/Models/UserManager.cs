using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CBSPay.API.App_Start;
using CBSPay.Core.Entities;
using Microsoft.AspNet.Identity;

namespace CBSPay.API.Models
{
    public class UserManager : UserManager<UserrRecord,int>
    {
        public UserManager()
        :
            base(new UserStore())
        {
            UserValidator = new UserValidator<UserrRecord,int>(this);
            PasswordValidator = new PasswordValidator();
        }
    }
}