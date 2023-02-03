using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class MDASettingsManager : BaseManager<MDASettings>, IMDASettingsManager<MDASettings>
    {
        private readonly IRepository<MDASettings> _settingsRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;

        public MDASettingsManager(IRepository<MDASettings> settingsRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(settingsRepository, user, orchardServices)
        {
            _settingsRepository = settingsRepository;
            _orchardServices = orchardServices;
        }
    }

    public class BankDetailsManager : BaseManager<BankDetails>, IBankDetailsManager<BankDetails>
    {
        private readonly IRepository<BankDetails> _bankDetailsRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;

        public BankDetailsManager(IRepository<BankDetails> bankDetailsRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(bankDetailsRepository, user, orchardServices)
        {
            _bankDetailsRepository = bankDetailsRepository;
            _orchardServices = orchardServices;
        }
    }
}