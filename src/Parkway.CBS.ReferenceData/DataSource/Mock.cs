using Newtonsoft.Json;
using Parkway.CBS.ReferenceData.Configuration;
using Parkway.CBS.ReferenceData.DataSource.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.DataSource
{
    public class Mock : BaseReferenceDataSource, IReferenceDataSource
    {
        private readonly IRefDataSystem _mockSystem;

        public Mock(IRefDataSystem mockSystem = null)
        {
            _mockSystem = mockSystem;
        }

        /// <summary>
        /// Source name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string</returns>
        public string ReferenceDataSourceName() => "Mock";


        public List<GenericRefDataTemp> GetActiveBillableTaxEntitesPerRevenueHead(int revenueHeadId, RefData refData, dynamic processDetails)
        {
            if(_mockSystem != null)
            {
                return _mockSystem.GetActiveBillableTaxEntitesPerRevenueHead(revenueHeadId, refData, processDetails);
            }
            try
            {
                List<GenericRefDataTemp> ents = new List<GenericRefDataTemp>();
                for (int i = 0; i < 1000000; i++)
                {
                    var taxId = Faker.RandomNumber.Next(100023, 983923434).ToString();
                    ents.Add(new GenericRefDataTemp
                    {
                        Address = Faker.Address.StreetAddress(true),
                        BatchNumber = processDetails.BatchNumber,
                        Recipient = Faker.Name.FullName(),
                        BillingModelId = processDetails.BillingId,
                        RevenueHeadId = revenueHeadId,
                        Email = i.ToString() + DateTime.Now.ToString() + Faker.Internet.Email(),
                        Status = 0,
                        TaxEntityCategoryId = Faker.RandomNumber.Next(1, 2),
                        TaxIdentificationNumber = taxId,
                        AdditionalDetails = JsonConvert.SerializeObject(new List<GenericAdditionalDetails> { { new GenericAdditionalDetails { IdentifierName = "ReferenceData", IdentifierValue = "Ref:1807#" + DateTime.Now.ToLocalTime().Ticks.ToString() } } }),
                        StatusDetail = "Acquired ref data.Process level 1.",
                        UniqueIdentifier = string.Format("{0}#{1}#{2}", processDetails.BatchNumber, taxId, i)
                    });
                }
                return ents;
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}