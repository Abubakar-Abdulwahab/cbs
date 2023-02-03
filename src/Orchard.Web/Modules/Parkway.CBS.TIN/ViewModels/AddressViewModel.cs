using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.TIN.ViewModels
{
    public class AddressViewModel
    {
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string PMB { get; set; }
        public string CO { get; set; }
        public string AdditionalInformation { get; set; }

        public string LGA { get; set; }
        public string Ward { get; set; }
    }
}