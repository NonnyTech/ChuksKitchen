namespace ChuksKitchen.Business.Dtos.Request
{
    public class AddCartDto
    {
        public Guid UserId { get; set; }
        public Guid FoodId { get; set; }
        public int Quantity { get; set; }
    }
}
