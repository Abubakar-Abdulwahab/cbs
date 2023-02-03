using System;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class ResetPasswordHandler : IResetPasswordHandler
    {
        private readonly Lazy<IVerificationCodeManager<VerificationCode>> _verRepo;
        private readonly IMembershipService _membershipService;
        private readonly IUserEventHandler _userEventHandler;

        public ILogger Logger { get; set; }

        public ResetPasswordHandler(Lazy<IVerificationCodeManager<VerificationCode>> verRepo, IMembershipService membershipService, IUserEventHandler userEventHandler)
        {
            _verRepo = verRepo;
            _membershipService = membershipService;
            _userEventHandler = userEventHandler;
            Logger = NullLogger.Instance;
        }



        public void ResetPassword(string token, string newPassword)
        {
            try
            {
                string decpToken = Util.LetsDecrypt(token);
                VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(decpToken);
                VerificationCode verCode = _verRepo.Value.Get(v => v.Id == enobj.VerId);

                IUser userInfo = _membershipService.GetUser(verCode.CBSUser.Email);

                if(userInfo != null)
                {
                    //reset password
                    _membershipService.SetPassword(userInfo, newPassword);
                    _userEventHandler.ChangedPassword(userInfo);
                    return;
                }
                else { throw new Exception("User info not found"); }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}