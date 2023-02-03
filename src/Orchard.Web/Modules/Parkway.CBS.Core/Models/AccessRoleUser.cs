using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class AccessRoleUser : CBSModel
    {
        public virtual AccessRole AccessRole { get; set; }

        public virtual UserPartRecord User { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        ///// <summary>
        /////(concat('A-',CONVERT([nvarchar](50),[AccessRole_Id]),'-U-',CONVERT([nvarchar](50),[User_Id])))
        ///// (concat('R-',CONVERT([nvarchar](50),[Role_Id]),'-U-',CONVERT([nvarchar](50),[User_Id]),'-T-',CONVERT([nvarchar](50),[ReportType])))
        ///// </summary>
    }
}