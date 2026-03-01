using Microsoft.AspNetCore.Identity;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Create Roles
            string[] roles = { "Admin", "Doctor", "Nurse" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin User
            if (await userManager.FindByEmailAsync("admin@hospital.com") == null)
            {
                var admin = new ApplicationUser
                {
                    FullName = "System Administrator",
                    UserName = "admin@hospital.com",
                    Email = "admin@hospital.com",
                    UserRole = "Admin",
                    IsActive = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@12345");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Sample Doctor
            if (await userManager.FindByEmailAsync("doctor@hospital.com") == null)
            {
                var doctor = new ApplicationUser
                {
                    FullName = "Dr. John Smith",
                    UserName = "doctor@hospital.com",
                    Email = "doctor@hospital.com",
                    UserRole = "Doctor",
                    Specialization = "General Medicine",
                    Department = "OPD",
                    IsActive = true
                };
                var result = await userManager.CreateAsync(doctor, "Doctor@12345");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(doctor, "Doctor");
            }

            // Seed Sample Nurse
            if (await userManager.FindByEmailAsync("nurse@hospital.com") == null)
            {
                var nurse = new ApplicationUser
                {
                    FullName = "Nurse Mary Jane",
                    UserName = "nurse@hospital.com",
                    Email = "nurse@hospital.com",
                    UserRole = "Nurse",
                    Department = "OPD",
                    IsActive = true
                };
                var result = await userManager.CreateAsync(nurse, "Nurse@12345");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(nurse, "Nurse");
            }
        }
    }
}