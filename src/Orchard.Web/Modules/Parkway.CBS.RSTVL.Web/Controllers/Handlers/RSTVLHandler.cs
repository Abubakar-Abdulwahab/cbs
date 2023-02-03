using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.RSTVL.Core.CoreServices.Contracts;
using Parkway.CBS.RSTVL.Core.Models;
using Parkway.CBS.RSTVL.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.RSTVL.Web.Controllers.Handlers
{
    public class RSTVLHandler : IRSTVLHandler
    {
        private readonly ICoreLicence _coreLicence;

        public RSTVLHandler(ICoreLicence coreLicence)
        {
            _coreLicence = coreLicence;
        }



        /// <summary>
        /// Save licence details
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="invoiceDetails"></param>
        /// <param name="taxEntityId"></param>
        public void SaveLicenceDetails(GenerateInvoiceStepsModel processStage, InvoiceGeneratedResponseExtn invoiceDetails, Int64 taxEntityId)
        {
            RSTVLicence licence = new RSTVLicence
            {
                RevenueHead = new CBS.Core.Models.RevenueHead { Id = invoiceDetails.RevenueHeadID },
                Invoice = new CBS.Core.Models.Invoice { Id = invoiceDetails.InvoiceId },
                MDA = new CBS.Core.Models.MDA { Id = invoiceDetails.MDAId },
                CustomerReference = (processStage.UserFormDetails == null || !processStage.UserFormDetails.Any()) ? null : processStage.ExternalRef,
                LGA = new CBS.Core.Models.LGA { Id = processStage.SelectedStateLGA },
                State = new CBS.Core.Models.StateModel { Id = processStage.SelectedState },
                TaxEntity = new CBS.Core.Models.TaxEntity { Id = taxEntityId },
                Year = processStage.SelectedYear,
            };

            _coreLicence.SaveDetails(licence);
        }
    }
}