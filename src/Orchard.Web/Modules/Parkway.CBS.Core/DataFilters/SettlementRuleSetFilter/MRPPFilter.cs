using Parkway.CBS.Core.DataFilters.SettlementRuleSetFilter.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.DataFilters.SettlementRuleSetFilter
{
    public class SettlementRuleSetFilter : ISettlementRuleSetFilter
    {

        public void FindParentSet(ISettlementRuleManager<SettlementRule> _settlementRepo, SettlementRule settlementRule)
        {
            bool parentFound = false;
            for (int i = settlementRule.SettlementHierarchyLevel; settlementRule.SettlementHierarchyLevel < 0 || parentFound; i--)
            {
                switch (i)
                {
                    case 8:
                        parentFound = ParentSetCheckForMRPPandPC(_settlementRepo, settlementRule);
                        break;
                    case 7:
                        parentFound = ParentSetCheckForMPPorPC(_settlementRepo, settlementRule);
                        break;
                    //case 6:
                    //    value = DoMR(_settlementRepo);
                    //    break;
                    //case 5:
                    //    value = DoMPPandPC(_settlementRepo);
                    //    break;
                    //case 4:
                    //    value = DoMPPorPC(_settlementRepo);
                    //    break;
                    //case 3:
                    //    value = DoM(_settlementRepo);
                    //    break;
                    //case 2:
                    //    value = DoPPandPC(_settlementRepo);
                    //    break;
                    //case 1:
                    //    value = DoPPorPC(_settlementRepo);
                    //    break;
                    default:
                        break;
                }
            }
            throw new NotImplementedException();
        }



        private bool ParentSetCheckForMPPorPC(ISettlementRuleManager<SettlementRule> settlementRepo, SettlementRule settlementRule)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// check for the set that contains the value for 
        /// a settlement rule that has an MDA, Revenue Head, Payment Provider and 
        /// Payment Channel
        /// <para>this check assumes that the rule has an MDA, Revenue head, Payment Provider and Payment Channel</para>
        /// </summary>
        /// <param name="settlementRepo"></param>
        /// <returns></returns>
        private bool ParentSetCheckForMRPPandPC(ISettlementRuleManager<SettlementRule> settlementRepo, SettlementRule settlementRule)
        {
            return false;
            ////3-5-{>0}-{>0} eg 3-5-1-5
            //var leftParent = settlementRepo.GetParentSettlements(settlementRule.MDA, settlementRule.RevenueHead, settlementRule.PaymentProvider_Id, 0);
            //var rightParent = settlementRepo.GetParentSettlements(settlementRule.MDA, settlementRule.RevenueHead, 0, settlementRule.PaymentChannel_Id);
            ////
            //SettlementRuleVM rightSettlementParent = rightParent.SingleOrDefault();
            //SettlementRuleVM leftSettlementParent = leftParent.SingleOrDefault();
            ////check the settlement combination has any parent

            //if(leftSettlementParent != null) { settlementRule.LeftParentSettlment = new SettlementRule { Id = leftSettlementParent.Id }; }
            //if(rightSettlementParent != null) { settlementRule.RightParentSettlement = new SettlementRule { Id = rightSettlementParent.Id }; }

            //return settlementRule.LeftParentSettlment != null || settlementRule.RightParentSettlement != null;
        }
    }

}