using Cinema.Business.Services;
using Cinema.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services, string connectionString)
        {
            services.AddDataAccess(connectionString);
            services.AddTransient<IdentityAuthService>();
            services.AddTransient<MovieService>();
            services.AddTransient<MovieCommentService>();
            services.AddTransient<MovieSessionService>();
            return services;
        }
    }
}
