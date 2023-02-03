using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    /// <summary>
    /// Create revenue head model
    /// </summary>
    public class CreateRevenueHeadsModel
    {
        public ICollection<RevenueHead> RevenueHeads { get; set; }

        /// <summary>
        /// Are the revenue head(s) subrevenue head(s)
        /// </summary>
        public bool IsSubRevenueHead { get; set; }

        /// <summary>
        /// The mda the revenue head(s) belong to
        /// </summary>
        [Required(ErrorMessage = "MDA field is required")]
        public int ParentMDAId { get; set; }

        /// <summary>
        /// The mda slug the revenue head(s) belong to
        /// </summary>
        public string ParentMDASlug { get; set; }

        /// <summary>
        /// The parent revenue head, if the revenue heads IsSubRevenueHead is true.
        /// </summary>
        public int ParentRevenueHeadId { get; set; }

        /// <summary>
        /// Authorized user email
        /// </summary>
        [Required(ErrorMessage = "The authorized user email is required")]
        public string UserEmail { get; set; }
    }

    public class CreateRevenueHeadResponseModel
    {
        public List<RevenueHeadResponseModel> RevenueHeads { get; set; }
    }

    public class RevenueHeadResponseModel
    {
        public int CBSId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int ParentMDAId { get; set; }
        public int ParentRevenueHeadId { get; set; }
        public string CashflowProductCode { get; set; }
        public Int64 CashflowId { get; set; }
    }
}