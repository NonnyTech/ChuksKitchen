using ChuksKitchen.Business.Repositories.IRepositories;
using ChuksKitchen.Data.DataContext;
using ChuksKitchen.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ChuksKitchen.Business.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public async Task<Cart?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Food)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
