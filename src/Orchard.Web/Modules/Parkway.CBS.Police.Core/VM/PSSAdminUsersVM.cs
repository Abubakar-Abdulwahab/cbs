using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSAdminUsersVM
    {
        public int Id { get; set; }

        public int RoleTypeId { get; set; }

        public string Fullname { get; set; }

        public CommandVM Command { get; set; }

        public int UserId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}