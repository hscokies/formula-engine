using System.Text;
using Domain.Users;
using Infrastructure.AppSettings;
using Infrastructure.AppSettings.Models;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPersistence(IConfiguration configuration)
        {
            return services
                .AddSettings<DbSettings>(configuration)
                .AddDataContext<IDataContext, DataContext>(seed: true)
                .AddDataContext<IReadOnlyDataContext, ReadOnlyDataContext>(QueryTrackingBehavior.NoTracking);
        }

        private IServiceCollection AddDataContext<TService, TContext>(QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll, bool seed = false)
            where TService : class
            where TContext : DbContext, TService
        {
            return services
                .AddDbContextFactory<TContext>((sp, options) =>
                {
                    var connectionString = sp.GetRequiredService<IOptions<DbSettings>>().Value.ConnectionString;

                    options.UseNpgsql(connectionString, b =>
                        {
                            b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                        })
                        .UseQueryTrackingBehavior(trackingBehavior)
                        .UseSnakeCaseNamingConvention();
                    
                })
                .AddScoped<TService, TContext>((sp) => sp.GetRequiredService<IDbContextFactory<TContext>>().CreateDbContext());
        }

        public IServiceCollection AddIdentity()
        {
            services.AddIdentityCore<User>(options =>
                {
                    options.Password.RequiredLength = 12;

                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DataContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddApiEndpoints();

            services.AddAuthentication(IdentityConstants.BearerScheme)
                .AddBearerToken(IdentityConstants.BearerScheme);

            services.AddAuthorization();

            return services;
        }
    }

    extension(IApplicationBuilder app)
    {
        public async Task ApplyMigrations()
        {
            await using var scope = app.ApplicationServices.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    
            if (dbContext.Database.IsRelational())
            {
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
