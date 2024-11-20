using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TaskOneDraft.Areas.Identity.Data;

namespace TaskOneDraft.Areas.Identity.Data
{
    //static class for database initialization
    public static class DbInitializer
    {
        //method to initialize roles and default admin user
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //get role manager service
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(); //get user manager service, using ApplicationUser

            //define roles to be created
            string[] roleNames = { "Lecturer", "Admin" };

            //create roles if they do not exist
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName)) //check if role exists
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName)); //create role if it does not exist
                }
            }

            //seed admin user details
            var adminEmail = "admin@cmcs.com"; //admin email
            var adminPassword = "Admin123@#"; //admin password

            //check if the admin user exists
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail, //set username as email
                    Email = adminEmail, //set email
                    EmailConfirmed = true //confirm email
                };

                //create admin user with the defined password
                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin"); //assign admin role to the user
                }
            }
        }
    }
}
