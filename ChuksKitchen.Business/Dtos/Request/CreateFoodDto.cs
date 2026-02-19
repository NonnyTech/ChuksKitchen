namespace ChuksKitchen.Business.Dtos.Request
{
    public class CreateFoodDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid? UserId { get; set; }
    }
}
