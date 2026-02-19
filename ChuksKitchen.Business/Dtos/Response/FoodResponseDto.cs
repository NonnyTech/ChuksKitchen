namespace ChuksKitchen.Business.Dtos.Response
{
    public class FoodResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = false;
    }
}
