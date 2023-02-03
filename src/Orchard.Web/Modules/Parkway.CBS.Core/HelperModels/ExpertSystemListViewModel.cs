using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ExpertSystemListViewModel
    {
        /// <summary>
        /// Flag to indicate if the state settings has been added ir not
        /// </summary>
        public bool ShowSetStateButton { get; set; }

        public List<ExpertSystemSettings> ExpertSystemSettings { get; set; }

        public int PagerSize { get; set; }

        public dynamic Pager { get; set; }

    }
}