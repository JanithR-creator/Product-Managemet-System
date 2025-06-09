
using Microsoft.EntityFrameworkCore;
using ProductService.AdapterEndPointController;
using ProductService.AdapterEndPointController.Impl;
using ProductService.Data;
using ProductService.Messaging;
using ProductService.Services;
using ProductService.Services.ServiceImpl;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Useful for environment-based deployments
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IProductService, ProductServiceImpl>();
            builder.Services.AddScoped<IAdapterEnpointHandler, AdapterEndpointHandler>();

            builder.Services.AddHostedService<CheckoutEventConsumer>();

            builder.Services.AddDbContext<AppDbContext>(
                options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();
            
            app.UseSwagger();
            app.UseSwaggerUI();

            //Automatically applies EF Core migrations at runtime. This ensures your database schema is always in sync with your latest model changes.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            app.MapControllers();
            app.Run();
        }
    }
}
