using ChuksKitchen.Data.Enum;

namespace ChuksKitchen.Business.Dtos.Request
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
