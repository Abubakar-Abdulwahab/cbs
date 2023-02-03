using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Seeds
{
    public class PaymentProviderSeeds : IPaymentProviderSeeds
    {
        private readonly IThirdPartyPaymentProviderManager<ThirdPartyPaymentProvider> _repo;

        public PaymentProviderSeeds(IThirdPartyPaymentProviderManager<ThirdPartyPaymentProvider> repo)
        {
            _repo = repo;
        }

        public void PopPaymentProviders()
        {
            List<ThirdPartyPaymentProvider> list = new List<ThirdPartyPaymentProvider> { };
            //list.Add(new ThirdPartyPaymentProvider
            //{
            //    Identifier = (int)PaymentProvider.NetPay,
            //    Name = PaymentProvider.NetPay.ToString(),
            //});
            //list.Add(new ThirdPartyPaymentProvider
            //{
            //    Identifier = (int)PaymentProvider.Remita,
            //    Name = PaymentProvider.Remita.ToString(),
            //});
            //list.Add(new ThirdPartyPaymentProvider
            //{
            //    Identifier = (int)PaymentProvider.PayDirect,
            //    Name = PaymentProvider.PayDirect.ToString(),
            //});
            //list.Add(new ThirdPartyPaymentProvider
            //{
            //    Identifier = (int)PaymentProvider.BankCollect,
            //    Name = PaymentProvider.BankCollect.ToString(),
            //});
           
            //list.Add(new ThirdPartyPaymentProvider
            //{
            //    Identifier = (int)PaymentProvider.NIBSS,
            //    Name = PaymentProvider.NIBSS.ToString(),
            //});
            

            
            _repo.SaveBundle(list);
        }
    }
}