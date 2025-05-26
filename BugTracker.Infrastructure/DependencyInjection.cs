using BugTracker.Application.Logger;
using BugTracker.Application.Repositories;
using BugTracker.Application.Services;
using BugTracker.Infrastructure.Data;
using BugTracker.Infrastructure.Identity;
using BugTracker.Infrastructure.Logger;
using BugTracker.Infrastructure.Repositories;
using BugTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace BugTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection"), 
                builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            //Adding Services
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient(typeof(IRepo<>), typeof(Repo<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IExceptionLogger, ExceptionLogger>();

            services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                var key = Encoding.UTF8.GetBytes(config["JWT:Key"]);
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidAudience = config["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                };
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = async (context) =>
                    {
                        context.HandleResponse();
                        if (context.AuthenticateFailure == null)
                        {
                            context.Response.StatusCode = 403;
                        }
                        else if (context.AuthenticateFailure != null)
                        {
                            context.Response.StatusCode = 401;
                        }
                    }
                };
            });

            services.AddHttpContextAccessor();
            return services;
        }
    }
}

