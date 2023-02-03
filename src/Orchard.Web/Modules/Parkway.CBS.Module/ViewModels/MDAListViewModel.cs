using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class MDAListViewModel
    {
        public IEnumerable<MDA> ListOfMDA { get; set; }
        public dynamic Pager { get; set; }
        public MDAIndexOptions Options { get; set; }
    }

    public class MDAIndexOptions
    {
        public string Search { get; set; }
        public MDAOrder Order { get; set; }
        public MDAFilter Filter { get; set; }

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