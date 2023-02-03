using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class PoliceOfficerSeedingVM
    {
        public string Name { get; set; }

        public string IdentificationNumber { get; set; }

        public Gender Gender { get; set; }

        public string IPPISNumber { get; set; }

        public int RankId { get; set; }

        public int CommandId { get; set; }

        public string AccountNumber { get; set; }

        public string BankCode { get; set; }

        public bool Error { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class PoliceOfficerResponse
    {
        /// <summary>
        /// check to see if there are any errors here, that is if the header values have been read to be in correct order
        /// </summary>
        public HeaderValidateObject HeaderValidateObject { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of officers
        /// </summary>
        public ConcurrentStack<PoliceOfficerSeedingVM> OfficerRecords { get; set; }
    }

    public class HeaderValidateObject
    {
        public bool Error { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class TemplateHeaderValidation
    {
        public bool HeaderPresent { get; set; }

        public string ErrorMessage { get; set; }

        public int IndexOnFile { get; set; }
    }

    public class PoliceOfficerStatVM
    {
        public int NumberofSuccessfulRecords { get; set; }

        public int NumberofUnSuccessfulRecords { get; set; }

        public List<PoliceOfficerSeedingVM> UnSuccessfulRecords { get; set; }

    }
}