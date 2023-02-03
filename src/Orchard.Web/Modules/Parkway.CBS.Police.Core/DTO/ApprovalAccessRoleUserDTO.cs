namespace Parkway.CBS.Police.Core.DTO
{
    public class ApprovalAccessRoleUserDTO
    {
        public int Id { get; set; }

        /// <summary>
        /// <see cref="Enums.AdminUserType"/>
        /// </summary>
        public int AccessType { get; set; }
    }
}