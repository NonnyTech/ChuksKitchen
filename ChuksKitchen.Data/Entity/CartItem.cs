namespace ChuksKitchen.Data.Entity
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid FoodId { get; set; }
        public int Quantity { get; set; }
        public Food Food { get; set; }
    }
}
