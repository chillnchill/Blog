using Microsoft.AspNetCore.Identity;

namespace Blog.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static IApplicationBuilder SeedAdministrator(this IApplicationBuilder app, string email)
        {
            //this allows us to take all service providers we need because the inversion of control is not available for
            //static classes

            //sync section
            using IServiceScope scopedServices = app.ApplicationServices.CreateScope();

            IServiceProvider serviceProvider = scopedServices.ServiceProvider;

            UserManager<IdentityUser> userManager =
                serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //async section
            Task.Run(async () =>
            {

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    IdentityRole adminRole = new IdentityRole("Admin");
                    await roleManager.CreateAsync(adminRole);
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    IdentityRole userRole = new IdentityRole("User");
                    await roleManager.CreateAsync(userRole);
                }

                if (await userManager.FindByNameAsync("admin") == null)
                {
                    IdentityUser adminUser = new IdentityUser()
                    {
                        UserName = "admin",
                        Email = email
                    };

                    await userManager.CreateAsync(adminUser, "password");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            })
            .GetAwaiter()
            .GetResult();

            return app;

        }
    }
}