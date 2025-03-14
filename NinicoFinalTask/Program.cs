using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Extensions;
using NinicoFinalTask.Helpers;
using NinicoFinalTask.Models;
using NinicoFinalTask.Services.Abstracts;
using NinicoFinalTask.Services.Implements;
using Stripe;
using System.Configuration;

namespace NinicoFinalTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<NinicoDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MsSql"));
            });
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                opt.Password.RequiredLength = 3;
                opt.SignIn.RequireConfirmedEmail = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Lockout.MaxFailedAccessAttempts = 100;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<NinicoDbContext>();

            builder.Services.AddScoped<IEmailService , EmailService>();


            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.Name));
           


            builder.Services.ConfigureApplicationCookie(x =>
            {
                x.AccessDeniedPath = "/Home/AccesDenied";
            });


            var app = builder.Build();
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseUserSeed();

            app.UseRouting();

            app.UseAuthorization();





            app.MapControllerRoute(name: "login",
             pattern: "login",
             defaults: new { controller = "Account", action = "login" }
               );



            app.MapControllerRoute(name: "register",
             pattern: "register",
             defaults: new { controller = "Account", action = "Register" }
               );
            app.MapControllerRoute(
           name: "areas",
           pattern: "{area:exists}/{controller=DashBoard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
