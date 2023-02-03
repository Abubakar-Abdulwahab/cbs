using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReportOptions
    {

        public IEnumerable<SelectListItem> Years { get; set; }
        public MDAOrderCriteria SortCriteria { get; set; }
        public MDAOrder OrderBy { get; set; }

        public string Search { get; set; }

        private bool _direction = false;
        public bool Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }
    }
}