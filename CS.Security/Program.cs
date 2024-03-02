using CS.Security.DataAccess;
using CS.Security.Interfaces;
using CS.Security.Models;
using CS.Security.Servises;
using CS.Security.Servises.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CS.Security
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbString"));
            });

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowMyOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var authConfig = new AuthSettings();
            builder.Configuration.GetSection("Auth").Bind(authConfig);
            builder.Services.AddSingleton(authConfig);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Auth:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Auth:Audience"],

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Auth:SecretKey"])),
                };
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseCors("AllowMyOrigins");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // seedin roles
            //using (var scope = app.Services.CreateScope())
            //{
            //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            //    var roles = new[] { "Admin", "Customer" };
            //    foreach (var role in roles)
            //    {
            //        if (!await roleManager.RoleExistsAsync(role))
            //            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            //    }
            //}

            app.Run();
        }
    }
}