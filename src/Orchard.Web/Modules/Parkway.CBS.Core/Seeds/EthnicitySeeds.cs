using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Seeds
{
    public class EthnicitySeeds : IEthnicitySeeds
    {
        private readonly IEthnicityManager<Ethnicity> _repo;

        public EthnicitySeeds(IEthnicityManager<Ethnicity> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Adds a collection of ethnicities
        /// </summary>
        /// <param name="ethnicities"></param>
        public void AddEthnicities(IEnumerable<dynamic> ethnicities)
        {
            try
            {
                List<Ethnicity> addedEthnicities = new List<Ethnicity> { };
                foreach (dynamic ethnicity in ethnicities)
                {
                    addedEthnicities.Add(new Ethnicity
                    {
                        Name = ethnicity.Name.ToString(),
                        IsActive = true
                    });
                }

                _repo.SaveBundle(addedEthnicities);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}