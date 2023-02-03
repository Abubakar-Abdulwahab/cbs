﻿using Orchard.Data;
using NHibernate.Cfg;
using Orchard.Utility;
using FluentNHibernate.Cfg;
using Parkway.CBS.Core.Models;
using FluentNHibernate.Automapping;

namespace Parkway.CBS.Core.Events
{
    public class PAYEBatchItemReceiptNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<PAYEBatchItemReceipt>(mapping => mapping.Map(x => x.ReceiptNumber, "ReceiptNumber").Column("ReceiptNumber").ReadOnly());
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }
    }
}