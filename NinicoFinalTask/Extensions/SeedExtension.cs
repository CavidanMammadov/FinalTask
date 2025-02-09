using Microsoft.AspNetCore.Identity;
using NinicoFinalTask.Enums;
using NinicoFinalTask.Models;

namespace NinicoFinalTask.Extensions
{
    public static class SeedExtension
    {
        public static void UseUserSeed(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!roleManager.Roles.Any())
                {
                    foreach (Roles item in Enum.GetValues(typeof(Roles)))
                    {
                        roleManager.CreateAsync(new IdentityRole(item.ToString())).Wait();

                    }
                }
                if (!userManager.Users.Any(x => x.NormalizedUserName == "ADMIN"))
                {
                    User u = new User
                    {
                        FullName = "admin",
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        ProfileImageUrl = "photo.jpg"
                    };
                    userManager.CreateAsync(u, "123").Wait();
                    userManager.AddToRoleAsync(u, nameof(Roles.Admin)).Wait();

                }
            }
        }
    }
}
