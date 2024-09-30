using CookingBlogService.Data;
using CookingBlogService.Middleware;
using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RecipeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddScoped<CookingBlogService.Services.RecipeService>();

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RequestTimeoutMiddleware>(TimeSpan.FromSeconds(10));

app.UseHttpsRedirection();
app.UseRouting();


app.UseCors("AllowAllOrigins");

app.MapControllers();

app.UseDiscoveryClient();

app.Run();