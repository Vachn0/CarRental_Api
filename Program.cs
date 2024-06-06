using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RentCar.data;
using RentCar.SMTP;
using SMTP;
using Bogus;
using RentCar.models;
using System.Text;
using RentCar.Faker;
using RentCar.Repository.Abstract;
using RentCar.Repository.Implementation;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
});

builder.Services.AddControllers();

// Configure Database Connection
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddTransient<SendMessage>();
builder.Services.AddTransient<SendRentMail>();
builder.Services.AddTransient<GetRentMail>();
builder.Services.AddTransient<IFileService, FileService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Seed Data (if needed)
// using (var scope = app.Services.CreateScope())
// {
//     var serviceProvider = scope.ServiceProvider;
//     // DataSeeder.SeedCars(serviceProvider, 1000);
//     // DataSeeder.SeedUsers(serviceProvider, 1000);
// }

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

// Ensure the "Uploads" directory exists
string uploadPath = Path.Combine(builder.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

// Configure Static File Serving
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadPath),
    RequestPath = "/Resources"
});

app.MapControllers();

app.Run();
