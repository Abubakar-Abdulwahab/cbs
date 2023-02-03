using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CBSPay.Core.Entities;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Services;
using Microsoft.AspNet.Identity;
using NHibernate;
using Parkway.Tools.NHibernate;

namespace CBSPay.API.App_Start
{
    public class UserStore : IUserStore<UserrRecord,int>, IUserPasswordStore<UserrRecord, int>, 
        IUserLockoutStore<UserrRecord, int>, IUserTwoFactorStore<UserrRecord, int>
    {
        private readonly IBaseRepository<UserrRecord> _userRepo;
        private readonly ISession session;
        public UserStore()
        {
           _userRepo = new Repository<UserrRecord>();
        }

        public Task CreateAsync(UserrRecord user)
        {
            // return Task.Run(() => session.SaveOrUpdate(user));
            return Task.Run(() => _userRepo.Insert(user));
        }

        public Task DeleteAsync(UserrRecord user)
        {
            //return Task.Run(() => session.Delete(user));
            return Task.Run(() => _userRepo.Delete(user));

        }

        public Task<UserrRecord> FindByIdAsync(int userId)
        {
            // return Task.Run(() => session.Get<UserrRecord>(userId));
            return Task.Run(() => _userRepo.Find(userId));
        }

        public Task<UserrRecord> FindByNameAsync(string userName)
        {
            return Task.Run(() =>
            {
                

                return _userRepo.Find(c => c.UserName == userName);
            });
        }
         
        public Task UpdateAsync(UserrRecord user)
        {
            return Task.Run(() => session.SaveOrUpdate(user));
        }

        public void Dispose()
        {
           // throw new NotImplementedException();
        }


        #region UserPassword Store
        public Task<int> GetAccessFailedCountAsync(UserrRecord user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(UserrRecord user)
        {
            return Task.FromResult(false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(UserrRecord user)
        {
            return Task.FromResult(DateTimeOffset.MaxValue);
        }

        public Task<string> GetPasswordHashAsync(UserrRecord user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> GetTwoFactorEnabledAsync(UserrRecord user)
        {
            return Task.FromResult(false);
        }

        public Task<bool> HasPasswordAsync(UserrRecord user)
        {
            return Task.FromResult(true);
        }

        public Task<int> IncrementAccessFailedCountAsync(UserrRecord user)
        {
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(UserrRecord user)
        {
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(UserrRecord user, bool enabled)
        {
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(UserrRecord user, DateTimeOffset lockoutEnd)
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(UserrRecord user, string passwordHash)
        {
            return Task.Run(() => user.PasswordHash = passwordHash);
        }

        public Task SetTwoFactorEnabledAsync(UserrRecord user, bool enabled)
        {
            return Task.CompletedTask;
        }

#endregion

    }

    public class RoleStore : IRoleStore<Role, int>
    {
        private readonly IBaseRepository<Role> _roleRepo;

        public RoleStore()
        {
            _roleRepo = new Repository<Role>();
        }
        public Task CreateAsync(Role role)
        {
            return Task.Run(() => _roleRepo.Insert(role));
        }

        public Task DeleteAsync(Role role)
        {
            return Task.Run(() => _roleRepo.Delete(role));
        }

        public void Dispose()
        {
           //
        }

        public Task<Role> FindByIdAsync(int roleId)
        {
            return Task.Run(() => _roleRepo.Find(roleId));
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            return Task.Run(()=>{
                return _roleRepo.Find(c => c.Name == roleName);
            });
        }

        public Task UpdateAsync(Role role)
        {
            return Task.Run(() => _roleRepo.Update(role));
        }
    }
}