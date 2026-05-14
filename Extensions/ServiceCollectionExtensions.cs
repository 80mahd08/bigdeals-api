using Microsoft.Extensions.DependencyInjection;
using api.Data.Connections;
using api.Helpers.Security;
using api.Interfaces.Auth;
using api.Interfaces.DemandesAnnonceur;
using api.Interfaces.Users;
using api.Repositories.Auth;
using api.Repositories.DemandesAnnonceur;
using api.Repositories.Users;
using api.Services.Auth;
using api.Services.DemandesAnnonceur;
using api.Services.Users;
using api.Interfaces.Categories;
using api.Repositories.Categories;
using api.Services.Categories;
using api.Interfaces.Annonces;
using api.Repositories.Annonces;
using api.Services.Annonces;
using api.Services.Storage;
using api.Interfaces.Favorites;
using api.Repositories.Favorites;
using api.Services.Favorites;
using api.Interfaces.Contacts;
using api.Repositories.Contacts;
using api.Services.Contacts;
using api.Interfaces.Email;
using api.Services.Email;
using api.Interfaces.Admin;
using api.Services.Admin;
using api.Interfaces.AnnonceurPayments;
using api.Repositories.AnnonceurPayments;
using api.Services.AnnonceurPayments;
using api.Repositories.Admin;
using api.Interfaces.Checkout;
using api.Repositories.Checkout;
using api.Services.Checkout;
using api.Interfaces.ProductPayments;
using api.Repositories.ProductPayments;
using api.Services.ProductPayments;
using api.Interfaces.Orders;
using api.Repositories.Orders;
using api.Services.Orders;
namespace api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Infrastructure
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddHttpContextAccessor();

        // Configuration
        services.Configure<api.Models.Config.FlouciSettings>(configuration.GetSection("Flouci"));

        // Helpers
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Repositories
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDemandeAnnonceurRepository, DemandeAnnonceurRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAnnonceRepository, AnnonceRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IAvisRepository, AvisRepository>();
        services.AddScoped<IAnnonceurPaymentRepository, AnnonceurPaymentRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<ICheckoutRepository, CheckoutRepository>();
        services.AddScoped<IProductPaymentRepository, ProductPaymentRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDemandeAnnonceurService, DemandeAnnonceurService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAnnonceService, AnnonceService>();
        services.AddScoped<ILocalFileStorageService, LocalFileStorageService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IAvisService, AvisService>();
        services.AddScoped<IAnnonceurPaymentService, AnnonceurPaymentService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IProductPaymentService, ProductPaymentService>();
        services.AddScoped<IOrdersService, OrdersService>();

        // External Services
        services.AddHttpClient<IFlouciAnnonceurPaymentService, FlouciAnnonceurPaymentService>();

        return services;
    }
}
