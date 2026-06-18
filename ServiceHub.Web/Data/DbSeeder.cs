using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Domain.Entities;

namespace ServiceHub.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Seed Roles
            string[] roles = { "Admin", "Employee", "SupportAgent", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Seed Departments
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new Department { Name = "IT Support" },
                    new Department { Name = "Human Resources" },
                    new Department { Name = "Finance & Accounting" },
                    new Department { Name = "Marketing & Sales" },
                    new Department { Name = "Operations" },
                    new Department { Name = "Customer Service" }
                );
                await context.SaveChangesAsync();
            }

            // 3. Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Hardware Issue" },
                    new Category { Name = "Software Request" },
                    new Category { Name = "Network Connectivity" },
                    new Category { Name = "Access Request" },
                    new Category { Name = "Printer/Scanner" },
                    new Category { Name = "Security & Data" }
                );
                await context.SaveChangesAsync();
            }

            // 4. Seed Support Agents
            var agents = new[]
            {
                new { Email = "sarah.j@servicehub.com", Password = "Agent@123" },
                new { Email = "mark.t@servicehub.com", Password = "Agent@123" },
                new { Email = "elena.r@servicehub.com", Password = "Agent@123" },
                new { Email = "david.c@servicehub.com", Password = "Agent@123" }
            };

            foreach (var agentData in agents)
            {
                if (await userManager.FindByEmailAsync(agentData.Email) == null)
                {
                    var agent = new IdentityUser { UserName = agentData.Email, Email = agentData.Email, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(agent, agentData.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(agent, "SupportAgent");
                    }
                }
            }
        }
    }
}