using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Filters;
using UserService.Data;
using UserService.Middleware;
using UserService.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddHealthChecks();

builder.Services.AddAuthorization();

builder.Services.AddControllers();


var app = builder.Build();

app.MapHealthChecks("/user/health");
// Add Prometheus middleware
app.UseHttpMetrics();
// Expose metrics endpoint at "/metrics"
app.MapMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RequestTimeoutMiddleware>(TimeSpan.FromSeconds(3));
app.UseMiddleware<HealthMonitoringMiddleware>();
app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();
app.UseRouting();


app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseDiscoveryClient();

app.Run();