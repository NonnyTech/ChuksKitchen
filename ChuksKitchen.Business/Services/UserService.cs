using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Repositories.IRepositories;
using ChuksKitchen.Data.Entity;
// Passwords are stored and compared as plain text per project requirement (not recommended for production)

namespace ChuksKitchen.Business.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        private readonly ReferralService _referralService;

        public UserService(IUserRepository repo, ReferralService referralService)
        {
            _repo = repo;
            _referralService = referralService;
        }

        public async Task<User> SignupAsync(SignupDto dto)
        {
            var existing = await _repo.GetByEmailOrPhoneAsync(dto.Email, dto.Phone);
            if (existing != null)
                throw new Exception("Duplicate email or phone");

            if (!string.IsNullOrWhiteSpace(dto.ReferralCode))
            {
                var valid = await _referralService.IsValidAsync(dto.ReferralCode);
                if (!valid)
                    throw new Exception("Invalid referral code");
            }

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password != dto.ConfirmPassword)
                throw new Exception("Password and confirm password must match");


            // generate a random 4-digit OTP
            var otp = new Random().Next(1000, 10000).ToString();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Phone = dto.Phone,
                ReferralCode = dto.ReferralCode,
                OtpCode = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(5),
                Role = "Admin",
                IsVerified = false
            };

            // store password as-is (plain text) per user instruction
            user.PasswordHash = dto.Password;

            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            return user;
        }

        public async Task<bool> VerifyAsync(VerifyDto dto)
        {
            var user = await _repo.GetByEmailOrPhoneAsync(dto.Email, null);
            if (user == null) return false;

            if (user.OtpCode != dto.Otp)
                return false;

            if (user.OtpExpiry.HasValue && user.OtpExpiry.Value < DateTime.UtcNow)
                return false;

            user.IsVerified = true;
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<User?> FindByEmailOrPhoneAsync(string emailOrPhone)
        {
            // try email first
            var byEmail = await _repo.GetByEmailOrPhoneAsync(emailOrPhone, null);
            if (byEmail != null) return byEmail;

            var byPhone = await _repo.GetByEmailOrPhoneAsync(null, emailOrPhone);
            return byPhone;
        }

        public async Task<User?> LoginAsync(string emailOrPhone, string password)
        {
            var user = await FindByEmailOrPhoneAsync(emailOrPhone);
            if (user == null) return null;

            // compare plain text password per user instruction
            if (user.PasswordHash == password)
                return user;

            return null;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }


}
