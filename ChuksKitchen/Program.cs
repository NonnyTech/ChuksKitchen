using ChuksKitchen.Data.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
// no additional OpenAPI package required; use built-in MapOpenApi

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<ChuksKitchen.Business.Repositories.IRepositories.IUserRepository, ChuksKitchen.Business.Repositories.UserRepository>();
builder.Services.AddScoped<ChuksKitchen.Business.Repositories.IRepositories.IFoodRepository, ChuksKitchen.Business.Repositories.FoodRepository>();
builder.Services.AddScoped<ChuksKitchen.Business.Repositories.IRepositories.IOrderRepository, ChuksKitchen.Business.Repositories.OrderRepository>();
builder.Services.AddScoped<ChuksKitchen.Business.Repositories.IRepositories.ICartRepository, ChuksKitchen.Business.Repositories.CartRepository>();


// Register services
builder.Services.AddScoped<ChuksKitchen.Business.Services.UserService>();
builder.Services.AddScoped<ChuksKitchen.Business.Services.ReferralService>();
builder.Services.AddScoped<ChuksKitchen.Business.Services.FoodService>();
builder.Services.AddScoped<ChuksKitchen.Business.Services.OrderService>();
builder.Services.AddScoped<ChuksKitchen.Business.Services.CartService>();
// Make enums serialize as strings in JSON responses
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Ensure ReferralService is provided to UserService
builder.Services.AddScoped<ChuksKitchen.Business.Services.UserService>(sp =>
    new ChuksKitchen.Business.Services.UserService(
        sp.GetRequiredService<ChuksKitchen.Business.Repositories.IRepositories.IUserRepository>(),
        sp.GetRequiredService<ChuksKitchen.Business.Services.ReferralService>()
    )
);

// ✅ Add DbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChuksKitchen API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();