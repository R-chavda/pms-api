namespace Application.DTOs.Response.User
{
    public class UserCreationMetadata
    {
        public List<UserResponseMinimal> Users { get; set; } = [];
        public List<RoleResponse> Roles{ get; set; } = []; 
    }
}
