using IntelliTest.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace IntelliTest.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder SeedAdmin(this IApplicationBuilder app)
        {
            string adminRoleName = "Admin";
            string adminEmail = "admin@email.com";
            using var scopedServices = app.ApplicationServices.CreateScope();
            var services = scopedServices.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            Task.Run(async () =>
                {
                    if (!(await roleManager.RoleExistsAsync(adminRoleName)))
                    {
                        var role = new IdentityRole { Name = adminRoleName };
                        await roleManager.CreateAsync(role);
                    }
                    var admin = await userManager.FindByNameAsync(adminEmail);

                    await userManager.AddToRoleAsync(admin, adminRoleName);
                })
                .GetAwaiter()
                .GetResult();

            return app;
        }

    }
}
