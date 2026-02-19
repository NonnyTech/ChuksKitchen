using ChuksKitchen.Data.Entity;

namespace ChuksKitchen.Business.Repositories.IRepositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task<List<Order>> GetAllAsync();
        Task UpdateAsync(Order order);
        Task SaveChangesAsync();

    }
}
