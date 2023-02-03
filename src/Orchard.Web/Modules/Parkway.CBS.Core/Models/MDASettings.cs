using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.Models
{
    public class MDASettings : CBSModel
    {
        [StringLength(250, ErrorMessage = "The company address value can only be 250 characters long")]
        public virtual string CompanyAddress { get; set; }

        [Required(ErrorMessage ="The Email field is required")]
        [StringLength(50, ErrorMessage = "The Email value can only be 250 characters long")]
        [EmailAddress(ErrorMessage ="Add a valid Email address")]
        public virtual string CompanyEmail { get; set; }

        [StringLength(14, ErrorMessage = "The Phone number value can only be 15 characters long", MinimumLength = 0)]
        public virtual string CompanyMobile { get; set; }
        public virtual string BusinessNature { get; set; }
        public virtual string LogoPath { get; set; }
        public virtual string SignaturePath { get; set; }

        public string GetThumbnail(string photoPath)
        {
            var segs = photoPath.Split('/');
            var fileName = segs[segs.Length - 1];
            fileName = "Thumbnail/" + fileName.Split(new string[] { ".png" }, StringSplitOptions.None)[0] + "_thumbnail.png";
            segs[segs.Length - 1] = fileName;
            return string.Join("/", segs);
        }
    }
}