namespace ChuksKitchen.Business.Dtos.Request
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
