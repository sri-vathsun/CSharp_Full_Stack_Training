using Microsoft.EntityFrameworkCore;
using EmployeeManagement.API.Data;
using EmployeeManagement.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<EmployeeService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();