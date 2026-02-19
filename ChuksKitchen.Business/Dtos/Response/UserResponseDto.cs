namespace ChuksKitchen.Business.Dtos.Response
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public bool IsVerified { get; set; }
        public string Role { get; set; }
        public UserResponseDto() { }
    }
}
