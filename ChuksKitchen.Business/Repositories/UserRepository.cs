using ChuksKitchen.Business.Repositories.IRepositories;
using ChuksKitchen.Data.DataContext;
using ChuksKitchen.Data.Entity;
using Microsoft.EntityFrameworkCore;


namespace ChuksKitchen.Business.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailOrPhoneAsync(string? email, string? phone)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    (email != null && x.Email == email) ||
                    (phone != null && x.Phone == phone));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
