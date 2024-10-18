using CookingBlogService.Data;
using CookingBlogService.Middleware;
using CookingBlogService.Redis;
using CookingBlogService.WebSockets;
using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RecipeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddScoped<CookingBlogService.Services.RecipeService>();
builder.Services.AddSingleton<RedisSubscriber>();

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RequestTimeoutMiddleware>(TimeSpan.FromSeconds(3));

// Enable WebSockets
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
};
app.UseWebSockets(webSocketOptions);

app.UseHttpsRedirection();
app.UseRouting();


app.UseCors("AllowAllOrigins");

app.MapControllers();

app.UseDiscoveryClient();

app.MapHealthChecks("/health");

app.Run();