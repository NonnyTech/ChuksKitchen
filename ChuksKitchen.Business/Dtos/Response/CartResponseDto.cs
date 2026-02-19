namespace ChuksKitchen.Business.Dtos.Response
{
    public class CartResponseDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
