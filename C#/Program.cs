using ConstructionCompany.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ConstructionCompany
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "ConstructionCompanyAPI",
                        ValidAudience = "ConstructionCompanyClient",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ONvVE0bKYIfyQH8wASR9C41lTqtPo2JjXofAY4l2q1KdsbE6J7QWFh8iUBj3CP0yKfUbqFo3J1vnXxIlDcr0dCsOTpQB9A6PqReQb8AxZhjwJGflXcT4sEN5WvFU6DiPSB8wcMAQaIKflxE9mYr6btNy4T1kDdZ3"))
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<LogUserActionAttribute>();

            builder.Services.AddDbContext<ConstructionCompanyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Server=localhost;Database=ConstructionCompany;Trusted_Connection=True;TrustServerCertificate=True;")));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
