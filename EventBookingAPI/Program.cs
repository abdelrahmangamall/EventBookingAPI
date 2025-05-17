
using EventBookingAPI.Data;
using EventBookingAPI.Models;
using EventBookingAPI.Services.Interfaces;
using EventBookingAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using EventBookingAPI.Helper;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
namespace EventBookingAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddControllers()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();

            // إعدادات الثقافات المدعومة
            var supportedCultures = new[] { "en", "ar" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            // إعدادات ملفات الثابتة (لتحميل الصور)
            builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

            // Add services to the container
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            

           


            // Configure JWT Authentication
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<JwtHelper>();

            // Add configuration validation
            builder.Services.AddOptions<JwtSettings>()
                .Bind(builder.Configuration.GetSection("Jwt"))
                .ValidateDataAnnotations()
                .Validate(jwtSettings =>
                {
                    if (string.IsNullOrWhiteSpace(jwtSettings.Key))
                        return false;
                    if (jwtSettings.Key.Length < 32)
                        return false;
                    return true;
                }, "JWT Key must be at least 32 characters long");

            // Verify configuration is loaded correctly
            var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
            if (jwtConfig == null || string.IsNullOrWhiteSpace(jwtConfig.Key))
            {
                throw new InvalidOperationException("JWT configuration is missing or invalid");
            }

            // Register services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseRequestLocalization(localizationOptions);


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.Initialize(services);
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
