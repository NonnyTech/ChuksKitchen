using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Repositories.IRepositories;
using ChuksKitchen.Data.Entity;

namespace ChuksKitchen.Business.Services
{
    public class FoodService
    {
        private readonly IFoodRepository _repo;

        public FoodService(IFoodRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Food>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Food> CreateAsync(CreateFoodDto dto)
        {
            var food = new Food
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                IsAvailable = true
            };

            await _repo.AddAsync(food);
            await _repo.SaveChangesAsync();

            return food;
        }

        public async Task<Food?> UpdateAsync(Guid id, UpdateFoodDto dto)
        {
            var food = await _repo.GetByIdAsync(id);
            if (food == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                food.Name = dto.Name;

            if (dto.Price.HasValue)
                food.Price = dto.Price.Value;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                food.Description = dto.Description;
            await _repo.UpdateAsync(food);
            await _repo.SaveChangesAsync();

            return food;
        }

        public async Task<Food?> SetAvailabilityAsync(Guid id, bool available)
        {
            var food = await _repo.GetByIdAsync(id);
            if (food == null) return null;
            food.IsAvailable = available;
            await _repo.UpdateAsync(food);
            await _repo.SaveChangesAsync();
            return food;
        }

        public async Task<List<Food>> GetAllAdminAsync()
        {
            return await _repo.GetAllAsync();
        }
    }
}
