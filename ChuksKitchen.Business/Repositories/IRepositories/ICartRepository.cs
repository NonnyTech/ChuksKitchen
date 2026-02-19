using ChuksKitchen.Data.Entity;

namespace ChuksKitchen.Business.Repositories.IRepositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task SaveChangesAsync();
    }
}
