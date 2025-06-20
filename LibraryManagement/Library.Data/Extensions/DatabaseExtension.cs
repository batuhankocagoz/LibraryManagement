using Library.Core.Repositories;
using Library.Core.UnitOfWork;
using Library.Data.Database;
using Library.Data.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Data.Extensions;

public static class DatabaseExtension
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection string doğrudan appsettings.json'dan alınıyor
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString),
                mysqlOptions =>
                {
                    mysqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });
        });

        // Generic repository ve UnitOfWork DI kaydı
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
    }
}
