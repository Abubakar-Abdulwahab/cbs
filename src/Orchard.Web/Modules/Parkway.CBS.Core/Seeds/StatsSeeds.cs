using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class StatsSeeds : IStatsSeeds
    {
        private readonly IInvoiceStatisticsManager<Stats> _repo;

        public StatsSeeds(IInvoiceStatisticsManager<Stats> repo)
        {
            _repo = repo;
            
        }
        public bool Populate2()
        {
            try
            {
                _repo.Populate2();
            }
            catch (Exception exception)
            {
                throw;
            }
            return true;
        }


        public bool Populate()
        {
            try
            {
                _repo.Populate();
            }
            catch (Exception exception)
            {

                throw;
            }
            return true;
        }

        public bool Truncate()
        {
            try
            {
                _repo.DeleteAll();
            }
            catch (Exception exception)
            {

                throw;
            }
            return true;
        }
        public bool DoConcats()
        {
            try
            {
                _repo.DoConcat();
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }
    }
}