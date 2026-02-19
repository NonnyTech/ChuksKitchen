using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Repositories.IRepositories;
using ChuksKitchen.Data.Entity;

namespace ChuksKitchen.Business.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IFoodRepository _foodRepo;

        public CartService(ICartRepository cartRepo, IFoodRepository foodRepo)
        {
            _cartRepo = cartRepo;
            _foodRepo = foodRepo;
        }

        public async Task<Cart> AddToCartAsync(AddCartDto dto)
        {
            var userId = dto.UserId;
            var foodId = dto.FoodId;

            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
                await _cartRepo.AddAsync(cart);
            }

            var food = await _foodRepo.GetByIdAsync(foodId);
            if (food == null || !food.IsAvailable)
                throw new Exception("Food not available");

            var existing = cart.Items.FirstOrDefault(i => i.FoodId == food.Id);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    FoodId = food.Id,
                    Quantity = dto.Quantity,
                    Food = food
                });
            }

            await _cartRepo.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart?> GetCartAsync(Guid userId)
        {
            return await _cartRepo.GetByUserIdAsync(userId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null) return;
            cart.Items.Clear();
            await _cartRepo.SaveChangesAsync();
        }
    }
}
