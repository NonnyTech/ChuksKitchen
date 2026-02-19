using ChuksKitchen.Business.Repositories.IRepositories;
using ChuksKitchen.Data.DataContext;
using ChuksKitchen.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ChuksKitchen.Business.Repositories
{
    public class FoodRepository : IFoodRepository
    {
        private readonly ApplicationDbContext _context;

        public FoodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Food>> GetAllAsync()
        {
            // Return only available foods for regular customer browsing
            return await _context.Foods
                .Where(f => f.IsAvailable)
                .ToListAsync();
        }

        public async Task<List<Food>> GetAllAdminAsync()
        {
            // Admin view: return all foods regardless of availability
            return await _context.Foods.ToListAsync();
        }

        public async Task<Food?> GetByIdAsync(Guid id)
        {
            return await _context.Foods.FindAsync(id);
        }

        public async Task AddAsync(Food food)
        {
            await _context.Foods.AddAsync(food);
        }

        public async Task UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
