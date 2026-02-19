namespace ChuksKitchen.Business.Dtos.Request
{
    public class SignupDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? ReferralCode { get; set; }
    }
}
