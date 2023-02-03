using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;

namespace Parkway.CBS.Payee.PayeeAdapters
{
    public class ParkwayPayeeAdapter : BasePayeeAdapter, IPayeeAdapter
    {
        internal List<TaxRuleSet> Rules = new List<TaxRuleSet>
        { { new TaxRuleSet { Amount = 300000, Percentage = 7 } }, { new TaxRuleSet { Amount = 300000, Percentage = 11 } }, { new TaxRuleSet { Amount = 500000, Percentage = 15 } }, { new TaxRuleSet { Amount = 500000, Percentage = 19 } }, { new TaxRuleSet{ Amount = 1600000, Percentage = 21} }, { new TaxRuleSet { Amount = 3200000, Percentage = 24 } }  };


        public GetPayeResponse GetPayeeModels(string filePath, string LGAFilePath, string stateName)
        { return GetPayees(filePath, LGAFilePath, stateName); }


        /// <summary>
        /// Get the break down of the payee tax amount
        /// <para>this value is let empty if the payee model prop HasError is true</para>
        /// </summary>
        /// <param name="payees"></param>
        /// <returns>PayeeAmountAndBreakDown</returns>
        public PayeeAmountAndBreakDown GetRequestBreakDown(List<PayeeAssessmentLineRecordModel> payees)
        {
            Parallel.ForEach(payees, (payee) =>
            {
                if (payee.GrossAnnualEarnings.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Exemptions.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Month.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Year.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Phone.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.LGA.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.TaxPayerName.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Address.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Email.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.TaxPayerName.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.TaxPayerTIN.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if(payee.AssessmentDate == null) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }

                var onePercent = Math.Round((payee.GrossAnnualEarnings.Value / 100), 2);

                var consolidatedRelief = Math.Round(((payee.GrossAnnualEarnings.Value * 20) / 100) + 200000, 2);
                var totalDeductions = Math.Round(consolidatedRelief + payee.Exemptions.Value, 2);
                var annualTaxableIncome = payee.GrossAnnualEarnings.Value - totalDeductions;

                payee.PayeeBreakDown = GetBreakDown(annualTaxableIncome, onePercent, Rules);
            });

            return new PayeeAmountAndBreakDown { TotalAmount = payees.Sum(x => x.PayeeBreakDown.Tax), Payees = payees };
        }


        public List<PayeeAssessmentLineRecordModel> GetPAYEModels<T>(ICollection<T> lines, string LGAFilePath, string stateName)
        {
            return GetPayees(lines as ICollection<DirectAssessmentPayeeLine>, LGAFilePath, stateName);
        }


        /// <summary>
        /// Get the response to reading the file and processing the file from the file path
        /// </summary>
        /// <typeparam name="PR">the Payee model 
        /// <see cref="IPPISPayeeResponse"/>
        /// <see cref="PayeeResponseModel"/>
        /// </typeparam>
        /// <param name="filePath"></param>
        /// <param name="LGAFilePath"></param>
        /// <param name="stateName"></param>
        /// <returns>IR</returns>
        public IR GetPayeeResponseModels<IR>(string filePath, string LGAFilePath, string stateName, int month = 0, int year = 0) where IR : GetPayeResponse
        {
            return GetPayees(filePath, LGAFilePath, stateName) as IR;
        }

    }
}
