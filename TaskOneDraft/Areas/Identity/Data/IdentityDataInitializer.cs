using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskOneDraft.Areas.Identity.Data;

public static class IdentityDataInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(); // Ensure correct type

        // Define roles
        string[] roleNames = { "Admin", "Lecturer" };

        // Create roles if they do not exist
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    Console.WriteLine($"Failed to create role: {roleName}");
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Role {roleName} already exists.");
            }
        }

        // Create a hardcoded admin user
        var adminEmail = "admin@cmcs.com";
        var adminPassword = "Admin@123@#";

        // Find or create the admin user
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (userResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!roleResult.Succeeded)
                {
                    Console.WriteLine($"Failed to assign Admin role to user: {roleResult.Errors.First().Description}");
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Failed to create admin user: {userResult.Errors.First().Description}");
                foreach (var error in userResult.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("Admin user already exists.");
        }
    }
}
