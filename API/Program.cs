using API.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;


internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<StoreContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
        builder.Services.AddAutoMapper(typeof(MappingProfiles));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseStatusCodePagesWithReExecute("/errors/{0}");
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthorization();
        app.MapControllers();

        await SeedDatabase(app);

        await app.RunAsync();
    }

    private static async Task SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<StoreContext>();
            await context.Database.MigrateAsync();
            await StoreContextSeed.SeedAsync(context, services.GetRequiredService<ILoggerFactory>());
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }

}