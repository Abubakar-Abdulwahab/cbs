using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateRevenueHeadRequestModel
    {
        public RevenueHead RevenueHead { get; set; }

        /// <summary>
        /// Is this revenue head a sub revenue head
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

        public string RequestReference { get; set; }
    }
}