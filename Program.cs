using Microsoft.Identity.Client;
using RestaurantAPI.Entities;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Services;
using NLog;
using NLog.Web;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;

namespace RestaurantAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Konfiguracja JWT
            var authSettings = new AuthSettings();

            builder.Configuration.GetSection("Auth").Bind(authSettings);

            builder.Services.AddSingleton(authSettings); // Dzieki temu mo¿emy wstrzykn¹æ to do servicu
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authSettings.JwtIssuer,
                    ValidAudience = authSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey)),
                };
            });
            builder.Services.AddAuthorization(option =>
            {
                option.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality"));
                option.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinAgeRequirement(20)));
            });
            builder.Services.AddScoped<IAuthorizationHandler, MinAgeRequirementHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            //Dodanie kontrolerów
            builder.Services.AddControllers();
            //Dodanie Bazy danych
            builder.Services.AddDbContext<RestaurantDBContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("RestaruantDb")));
            //Zarejestrowanie seedera
            builder.Services.AddScoped<RestaurantSeederService>();
            //Zarejestrowanie RestaurantService
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IDishService, DishService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            //Dodanie mappera
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //Dodanie Hasshera
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            //Dodanie FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            builder.Services.AddScoped<IValidator<GetAllRestaruantQuery>, RestaruantQuerryValidator>();
            //Dodanie Nloga
            builder.Host.UseNLog();
            //Dodanie Middleware ErrorHandlingMiddleware
            builder.Services.AddScoped<ErrorHandlingMiddleware>();

            // Dziêki temu mo¿emy mieæ kontekst usera poza kontrolerami
            builder.Services.AddScoped<IUserContextService, UserContextService>();
            builder.Services.AddHttpContextAccessor();

            var allowedOrigins = builder.Configuration["AllowedOrigins"];

            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
                options.AddPolicy("FrontendClient", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithOrigins(allowedOrigins)
                )
            );

            var app = builder.Build();
            app.UseResponseCaching();
            app.UseStaticFiles();

            app.UseCors("FrontendClient");

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication(); // Aplikacja u¿ywa autetykacji
            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseSwagger(); // U¿ycie swaggera
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API"));
            app.UseRouting();

            app.UseAuthorization(); // Aplikacja korzysta z autoryzacji

            app.MapControllers();

            SeedDatabase(); // Zasedowanie DB je¿eli jest pusta
            app.Run();

            void SeedDatabase() //can be placed at the very bottom under app.Run()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<RestaurantSeederService>();
                    dbInitializer.Seed();
                }
            }
        }
    }
}