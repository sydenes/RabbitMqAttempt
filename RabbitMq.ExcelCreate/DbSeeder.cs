using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.ExcelCreate.Models;

namespace RabbitMq.ExcelCreate
{
    public static class DbSeeder
    {
        public static void DbSeederExtension(this IServiceCollection services)
        {
            var appDbContext=services.BuildServiceProvider().GetRequiredService<AppDbContext>();
            var userManager=services.BuildServiceProvider().GetRequiredService<UserManager<IdentityUser>>();
            appDbContext.Database.Migrate(); //If the database does not exist, it creates it. From migration files, it creates tables in the database.

            if(!appDbContext.Users.AnyAsync().Result)
            {
                userManager.CreateAsync(new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    }, "Test123*").Wait();
                userManager.CreateAsync(new IdentityUser
                {
                    UserName = "user",
                    Email = "user@gmail.com",
                    }, "User123*").Wait();
            }
        }
       
    }
}
