using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IResetPasswordHandler : IDependency
    {
        /// <summary>
        /// Reset password attached to user data encrypted in the specified token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newpassword"></param>
        void ResetPassword(string token, string newpassword);
    }
}
