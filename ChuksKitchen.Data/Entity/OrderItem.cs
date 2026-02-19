namespace ChuksKitchen.Data.Entity
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid FoodId { get; set; }
        public int Quantity { get; set; }
        public Food Food { get; set; }
    }
}
