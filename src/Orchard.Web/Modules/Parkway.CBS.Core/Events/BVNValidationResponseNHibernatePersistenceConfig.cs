using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Events
{
    public class BVNValidationResponseNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg) { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<BVNValidationResponse>(mapping => mapping.Map(x => x.Base64Image).Length(int.MaxValue));
            defaultModel.Override<BVNValidationResponse>(mapping => mapping.Map(x => x.ResponseDump).Length(int.MaxValue));
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }
    }
}