using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class FormControlsSeeds : IFormControlsSeeds
    {
        IFormControlsManager<FormControl> _formRepository;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public FormControlsSeeds(IFormControlsManager<FormControl> formRepository)
        {
            _formRepository = formRepository;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Adds records to the forms control table for update 1
        /// <para>Returns int 2</para>
        /// </summary>
        /// <returns>int</returns>
        public void Seed1()
        {
            ICollection<FormControl> formMigrations = new List<FormControl>();

            formMigrations.Add(new FormControl()
            {
                ControlTypeNumber = (int)ControlTypes.TextBox,
                FriendlyName = T("Tax Identification Number").ToString(),
                HintText = T("Enter your Tax Identification Number").ToString(),
                DefaultStatus = true,
                LabelText = "Tax Identification Number",
                Name = "Tax Identification Number",
                PlaceHolderText = T("Enter your TIN/RIN").ToString(),
                //Validators = "Text-Required,MaxLength:500,MinLength:2",
                //TaxEntityCategory = new TaxEntityCategory { Id = 1 },
                //IsCompulsory = true,
                TechnicalName = "TaxIdentificationNumber",
            });

            formMigrations.Add(new FormControl()
            {
                ControlTypeNumber = (int)ControlTypes.TextBox,
                FriendlyName = T("Reference Number").ToString(),
                HintText = T("Enter your Reference Number").ToString(),
                LabelText = "Reference Number",
                Name = "Reference Number",
                PlaceHolderText = T("Enter your Reference Number").ToString(),
                //Validators = "Text-Required,MaxLength:500,MinLength:2",
                //TaxEntityCategory = new TaxEntityCategory { Id = 1 },
                //IsCompulsory = true,
                TechnicalName = "ReferenceNumber",
            });
            
            if (_formRepository.SaveBundle(formMigrations))
            {
                Logger.Error("Seed Error Update from 2: Error seeding Form Controls table");
            }
        }
    }
}