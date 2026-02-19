namespace ChuksKitchen.Business.Dtos.Request
{
    public class OrderItemDto
    {
        public Guid FoodId { get; set; }
        public int Quantity { get; set; }
    }
}
