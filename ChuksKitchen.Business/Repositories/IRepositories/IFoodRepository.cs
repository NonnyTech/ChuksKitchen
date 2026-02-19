using ChuksKitchen.Data.Entity;

namespace ChuksKitchen.Business.Repositories.IRepositories
{
    public interface IFoodRepository
    {
        Task<List<Food>> GetAllAsync();
        Task<Food?> GetByIdAsync(Guid id);
        Task AddAsync(Food food);
        Task UpdateAsync(Food food);
        Task SaveChangesAsync();
        // Optional: method to fetch all regardless of availability for admin
        Task<List<Food>> GetAllAdminAsync();
    }
}
