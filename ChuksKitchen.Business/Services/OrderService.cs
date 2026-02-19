using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Repositories.IRepositories;
using ChuksKitchen.Data.Entity;

namespace ChuksKitchen.Business.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IFoodRepository _foodRepo;

        public OrderService(IOrderRepository repo, IFoodRepository foodRepo)
        {
            _repo = repo;
            _foodRepo = foodRepo;
        }

        public async Task<Order> CreateAsync(CreateOrderDto dto)
        {
            decimal total = 0;
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId, // This line is unchanged
                Status = Data.Enum.OrderStatus.Pending
            };

            foreach (var item in dto.Items)
            {
                var food = await _foodRepo.GetByIdAsync(item.FoodId);
                if (food == null || !food.IsAvailable)
                    throw new Exception($"Food item {item.FoodId} is not available");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    FoodId = food.Id,
                    Quantity = item.Quantity,
                    Food = food
                };

                order.Items.Add(orderItem);
                total += food.Price * item.Quantity;
            }

            order.TotalPrice = total;

            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Order?> UpdateStatusAsync(Guid orderId, Data.Enum.OrderStatus status)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return null;

            order.Status = status;
            await _repo.UpdateAsync(order);
            await _repo.SaveChangesAsync();

            return order;
        }

        public async Task<bool> CancelAsync(Guid orderId)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = Data.Enum.OrderStatus.Cancelled;
            await _repo.UpdateAsync(order);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelByCustomerAsync(Guid orderId)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return false;

            // Allow customer cancel only when order is Pending or Confirmed
            if (order.Status == Data.Enum.OrderStatus.Pending || order.Status == Data.Enum.OrderStatus.Confirmed)
            {
                order.Status = Data.Enum.OrderStatus.Cancelled;
                await _repo.UpdateAsync(order);
                await _repo.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
