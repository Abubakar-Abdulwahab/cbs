using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class MDARevenueHeadsListPage
    {
        public MDA Mda { get; set; }
        public List<RevenueHead> RevenueHeads { get; set; }
        public dynamic Pager { get; set; }
        public RHIndexOptions Options { get; set; }
    }

    public class RHIndexOptions
    {
        public string Search { get; set; }
        public RevHeadOrder Order { get; set; }
        public RevHeadFilter Filter { get; set; }
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

    public enum RevHeadOrder
    {
        UpdatedAtUtc,
        CreatedAtUtc,
        Name,
        Code,
    }

    public enum RevHeadFilter
    {
        All,
        Enabled,
        Disabled,
    }
}