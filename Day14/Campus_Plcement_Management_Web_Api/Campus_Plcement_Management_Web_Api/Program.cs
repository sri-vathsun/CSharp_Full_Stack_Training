
using Campus_Plcement_Management_Web_Api.Data;
using Campus_Plcement_Management_Web_Api.Repositories.Interfaces;
using Campus_Plcement_Management_Web_Api.Models;
using CampusPlacement.API.Repositories.Implementations;
using Campus_Plcement_Management_Web_Api.Services.Implementations;
using Campus_Plcement_Management_Web_Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Configure SQL Server and DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// 3. Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 4. Register Repository
builder.Services.AddScoped<
    ICollegeRepository,
    CollegeRepository>();

// 5. Register Service
builder.Services.AddScoped<
    ICollegeService,
    CollegeService>();

// 6. Add API documentation
builder.Services.AddOpenApi();

var app = builder.Build();

// 7. Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
