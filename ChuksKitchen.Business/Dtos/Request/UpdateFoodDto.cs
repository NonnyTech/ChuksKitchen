namespace ChuksKitchen.Business.Dtos.Request
{
    public class UpdateFoodDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
    }
}
