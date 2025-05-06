using System.Text;
using FluentValidation.AspNetCore;
using LMS.Helpers;
using LMS.Mapping;
using LMS.Repositories.Implements;
using LMS.Repositories.Interfaces;
using LMS.Services.Implements;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;

namespace LMS
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBookBorrowingRequestRepository, BookBorrowingRequestRepository>();
            services.AddScoped<IBookBorrowingRequestService, BookBorrowingRequestService>();
            services.AddScoped<IBookBorrowingRequestDetailsRepository, BookBorrowingRequestDetailsRepository>();
            // services.AddScoped<IBookBorrowingRequestDetailsService, BookBorrowingRequestDetailsService>();
            services.AddScoped<ISendEmailSerivce, SendEmailService>();
            services.AddScoped<IHashPasswordHelper,HashPasswordHelperWrapper>();

            return services;
        }
        public static IServiceCollection AddValidationService(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddFluentValidationAutoValidation( options =>{
                options.DisableDataAnnotationsValidation = true;
            });
            return services;
        }
        public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtKey = Environment.GetEnvironmentVariable("JWTSETTINGS__KEY");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SUPER_USER", policy => policy.RequireRole("SUPER_USER"));
                options.AddPolicy("NORMAL_USER", policy => policy.RequireRole("NORMAL_USER"));
            });


            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "LMS API", Version = "v1", Description = "LMS API" });

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token in the following format: Bearer {your token}"
                });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

            });
            return services;
        }
        
    }
}
