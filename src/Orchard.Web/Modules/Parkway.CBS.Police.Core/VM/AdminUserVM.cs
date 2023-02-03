namespace Parkway.CBS.Police.Core.VM
{
    public class AdminUserVM
    {
        public string CommandCategoryName { get; set; }

        public int CommandCategoryId { get; set; }

        public string CommandName { get; set; }

        public bool IsActive { get; set; }

        public string Fullname { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string RoleName { get; set; }

        public string PhoneNumber { get; set; }

        public int CommandId { get; set; }

        public int RoleTypeId { get; set; }

        public int UserPartRecordId { get; set; }

        public int LastUpdatedById { get; set; }

        public int Id { get; set; }
    }
}