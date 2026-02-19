using ChuksKitchen.Data.Entity;

namespace ChuksKitchen.Business.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailOrPhoneAsync(string? email, string? phone);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
