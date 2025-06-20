using Library.Core.Repositories;
using Library.Core.Services;
using Library.Core.UnitOfWork;
using Library.Data.GenericRepository;
using Library.Data.UnitOfWork;
using Library.Service.Mapping;
using Library.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Service.Extensions;

public static class ServiceExtension
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(LibraryManagementMapping));

        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<AuthService>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

    }
}