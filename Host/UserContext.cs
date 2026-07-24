using Domain.Interfaces;

namespace Host
{
    public class UserContext : IUserContext
    {
        public bool IsAuthenticated { get; set; } = false;
        public long UserKeyId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
    }
}
