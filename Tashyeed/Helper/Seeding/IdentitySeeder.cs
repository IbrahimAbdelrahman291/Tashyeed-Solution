using Microsoft.AspNetCore.Identity;
using Tashyeed.Infrastructure.Identity;

namespace Tashyeed.Web.Helper.Seeding
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = ["Admin", "ProjectManager", "PurchasingManager","Enginner", "Supervisor","Accountant"]; 

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var adminUserName = "ENGHeshamGouda@Tashyeed.com";
            var adminUserName2 = "ENGIslamGouda@Tashyeed.com";
            if (await userManager.FindByEmailAsync(adminUserName) is not null)
                return;

            var admin1 = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminUserName,
                FullName = "Super Admin",
                EmailConfirmed = true
            };


            await userManager.CreateAsync(admin1, "HP@$$w0rd");
            await userManager.AddToRoleAsync(admin1, "Admin");

            if (await userManager.FindByEmailAsync(adminUserName2) is not null)
                return;

            var admin2 = new ApplicationUser
            {
                UserName = adminUserName2,
                Email = adminUserName2,
                FullName = "Super Admin",
                EmailConfirmed = true
            };


            await userManager.CreateAsync(admin2, "IP@$$w0rd");
            await userManager.AddToRoleAsync(admin2, "Admin");
        }

    }
}
