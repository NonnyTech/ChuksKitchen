namespace ChuksKitchen.Business.Services
{
    public class ReferralService
    {
        // Simple simulation: any non-empty code that equals "TRUEMINDS" is valid
        public Task<bool> IsValidAsync(string? code)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(code) && code == "TRUEMINDS");
        }
    }
}
