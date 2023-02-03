using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.PSSServiceType.ServiceOptions.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class ServiceOptionHandler : IServiceOptionHandler
    {
        private readonly ICoreServiceOptions _coreServiceOptions;
        private readonly IEnumerable<Lazy<IServiceOptionPresentation>> _optionPresentationImpl;

        public ILogger Logger { get; set; }

        public ServiceOptionHandler(ICoreServiceOptions coreServiceOptions, IEnumerable<Lazy<IServiceOptionPresentation>> optionPresentationImpl)
        {
            Logger = NullLogger.Instance;
            _coreServiceOptions = coreServiceOptions;
            _optionPresentationImpl = optionPresentationImpl;
        }


        /// <summary>
        /// Get options for PCC
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>PSSCharacterCertificateOptionsPageVM</returns>
        public PSServiceOptionsPageVM GetOptionsVM(int serviceId)
        {
            return new PSServiceOptionsPageVM
            {
                HeaderObj = new HeaderObj { },
                Options = _coreServiceOptions.GetActiveOtpions(serviceId)
            };
        }


        /// <summary>
        /// Validate and get selected option
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="selectedOption"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public PSServiceOptionsVM GetSelectedOption(int serviceId, int? selectedOption)
        {
            if (selectedOption.HasValue)
            {
                return _coreServiceOptions.GetActiveServiceOption(serviceId, selectedOption.Value);
            }
            return null;
        }


        /// <summary>
        /// Get the route name for this option
        /// </summary>
        /// <param name="serviceOption"></param>
        /// <returns>string</returns>
        public RouteNameAndStage GetNextOptionDirection(PSServiceOptionsVM serviceOption)
        {
            foreach (var option in _optionPresentationImpl)
            {
                if (option.Value.GetOptionType == serviceOption.OptionType)
                {
                    return option.Value.GetRouteName;
                }
            }
            throw new NotImplementedException("No option type found");
        }

    }
}