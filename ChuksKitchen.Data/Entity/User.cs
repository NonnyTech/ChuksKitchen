namespace ChuksKitchen.Data.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ReferralCode { get; set; }
        public bool IsVerified { get; set; }
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public string? Role { get; set; }
        // Password hash for authentication
        public string? PasswordHash { get; set; }
    }
}
