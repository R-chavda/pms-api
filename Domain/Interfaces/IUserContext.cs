namespace Domain.Interfaces
{
    public interface IUserContext
    {
        public bool IsAuthenticated { get; set; }
        public long UserKeyId { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
