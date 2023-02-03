using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.ThirdParty.Payment.Processor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class TranxLogSeeds : ITranxLog
    {
        ITransactionLogManager<TransactionLog> _repository;
        public ILogger Logger { get; set; }

        public TranxLogSeeds(ITransactionLogManager<TransactionLog> repository)
        {
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public void CorrectPaydirectBankCodeIssue()
        {
            try
            {
                var payDirects = _repository.GetCollection(x => x.Channel == 3);
                var banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
                Logger.Error("Correcting");
                foreach (var item in payDirects)
                {
                    var paymentObj = JsonConvert.DeserializeObject<Payment>(item.RequestDump);
                    //get the bank code
                    var bankCode = paymentObj.BankCode;
                    Logger.Error("Correcting " + bankCode);

                    //map bank code to what we have on file
                    //get CBN bank code
                    var fileBankCode = banks.Where(c => c.PayDirectBankCode == paymentObj.BankCode).FirstOrDefault();
                    if(fileBankCode != null)
                    {
                        Logger.Error("Correcting " + bankCode + " | " + fileBankCode.Code);
                        paymentObj.BankCode = fileBankCode.Code;
                        item.BankCode = fileBankCode.Code;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}