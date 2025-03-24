using ManageCoure.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace ManageCoure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var connectionString = configuration.GetConnectionString("DBContext");
            builder.Services.AddDbContext<PrnassmContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            builder.Services.AddScoped<PrnassmContext>();
            builder.Services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.Cookie.HttpOnly = true;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Studdent", policy => policy.RequireRole("Studdent"));
                options.AddPolicy("Tearcher", policy => policy.RequireRole("Tearcher"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
