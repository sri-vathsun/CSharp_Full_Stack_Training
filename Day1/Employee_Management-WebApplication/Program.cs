using Employee_Management_WebApplication.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Define CORS policy name
var allowFrontend = "_allowFrontend";

// 2. Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowFrontend, policy =>
    {
        policy.AllowAnyOrigin()   // Allows requests from VS Code Live Server / local frontend
              .AllowAnyHeader()   // Allows headers like Content-Type
              .AllowAnyMethod();  // Allows GET, POST, PUT, DELETE
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseInMemoryDatabase("EmployeeDatabase"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// 3. Enable CORS middleware (MUST be placed before app.MapControllers())
app.UseCors(allowFrontend);

app.MapControllers();

app.Run();