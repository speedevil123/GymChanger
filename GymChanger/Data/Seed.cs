using GymChanger.Models;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlTypes;

namespace GymChanger.Data
{
    public class Seed
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                if (!await roleManager.RoleExistsAsync(UserRoles.Trainer))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Trainer));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "syutkin_04@mail.ru";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        Name = "Никита",
                        Surname = "Сюткин",
                        MiddleName = "Юрьевич",
                        UserName = "speedevil",
                        BirthDay = "10.02.2004",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAdminUser, "Coding1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string appUserEmail = "appUser@mail.ru";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new AppUser()
                    {
                        Name = "Иван",
                        Surname = "Иванов",
                        MiddleName = "Иванович",
                        BirthDay = "25.03.1998",
                        UserName = "app-user1",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAppUser, "Coding1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }

                string trainerUserEmail = "trainerUser@mail.ru";

                var trainerUser = await userManager.FindByEmailAsync(trainerUserEmail);
                if (trainerUser == null)
                {
                    var newTrainerUser = new AppUser()
                    {
                        Name = "Алексей",
                        Surname = "Крюк",
                        MiddleName = "Евгеньевич",
                        BirthDay = "09.10.2003",
                        UserName = "trainer-user1",
                        Email = trainerUserEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newTrainerUser, "Coding1234?");
                    await userManager.AddToRoleAsync(newTrainerUser, UserRoles.Trainer);
                }

                string trainerUser2Email = "trainerUser2@mail.ru";

                var trainerUser2 = await userManager.FindByEmailAsync(trainerUser2Email);
                if (trainerUser2 == null)
                {
                    var newTrainerUser2 = new AppUser()
                    {
                        Name = "Дмитрий",
                        Surname = "Голубочкин",
                        MiddleName = "Владимирович",
                        BirthDay = "09.02.1995",
                        UserName = "trainer-user2",
                        Email = trainerUser2Email,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newTrainerUser2, "Coding1234?");
                    await userManager.AddToRoleAsync(newTrainerUser2, UserRoles.Trainer);
                }
            }
        }
    }
    public class Delete
    {
        public static void DeleteTable(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureDeleted();
                context.SaveChanges();
            }
        }

        public static void ClearTable(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                if (context.Courses.Any())
                {
                    context.Courses.RemoveRange(context.Courses);
                    context.SaveChanges();
                }
                else
                {
                    SqlNullValueException sqlnullvalue = new SqlNullValueException("Таблица уже пустая");
                    throw sqlnullvalue;
                }
            }
        }
    }
}

