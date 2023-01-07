using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StoreProject.Data;

namespace StoreProject.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange
                    (
                        new IdentityRole
                        {
                            Id = "2c5e174e - 3b0e - 446f - 86af483d56fd7200",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new IdentityRole
                        {
                            Id = "2c5e174e - 3b0e - 446f - 86af483d56fd7201",
                            Name = "Colaborator",
                            NormalizedName = "COLABORATOR"
                        },
                        new IdentityRole
                        {
                            Id = "2c5e174e - 3b0e - 446f - 86af483d56fd7202",
                            Name = "User",
                            NormalizedName = "USER"
                        }
                    );

                    var hasher = new PasswordHasher<ApplicationUser>();
                    context.Users.AddRange
                        (
                            new ApplicationUser
                            {
                                Id = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                UserName = "admin@localhost",
                                NormalizedUserName = "ADMIN@LOCALHOST",
                                Email = "admin@localhost",
                                NormalizedEmail = "ADMIN@LOCALHOST",
                                EmailConfirmed = true,
                                PasswordHash = hasher.HashPassword(null, "Admin123!"),
                            },
                            new ApplicationUser
                            {
                                Id = "2c5e174e - 3b0e - 446f - 86af483d56fd7204",
                                UserName = "colaborator@localhost",
                                NormalizedUserName = "COLABORATOR@LOCALHOST",
                                Email = "colaborator@localhost",
                                NormalizedEmail = "COLABORATOR@LOCALHOST",
                                EmailConfirmed = true,
                                PasswordHash = hasher.HashPassword(null, "Colaborator123!"),
                            },
                            new ApplicationUser
                            {
                                Id = "2c5e174e - 3b0e - 446f - 86af483d56fd7205",
                                UserName = "user@localhost",
                                NormalizedUserName = "USER@LOCALHOST",
                                Email = "user@localhost",
                                NormalizedEmail = "USER@LOCALHOST",
                                EmailConfirmed = true,
                                PasswordHash = hasher.HashPassword(null, "User123!")
                            }
                        );

                    context.UserRoles.AddRange
                        (
                            new IdentityUserRole<string>
                            {
                                RoleId = "2c5e174e - 3b0e - 446f - 86af483d56fd7200",
                                UserId = "2c5e174e - 3b0e - 446f - 86af483d56fd7203"
                            },
                            new IdentityUserRole<string>
                            {
                                RoleId = "2c5e174e - 3b0e - 446f - 86af483d56fd7201",
                                UserId = "2c5e174e - 3b0e - 446f - 86af483d56fd7204"
                            },
                            new IdentityUserRole<string>
                            {
                                RoleId = "2c5e174e - 3b0e - 446f - 86af483d56fd7202",
                                UserId = "2c5e174e - 3b0e - 446f - 86af483d56fd7205"
                            }
                        );

                }

                if (!context.Categories.Any())
                {
                    context.Categories.AddRange
                        (
                            new Category
                            {
                                CategoryID = "3ec2a986-f612-46c5-bdce-11c6d48fbf4f",
                                Name = "Telefoane"
                            },
                            new Category
                            {
                                CategoryID = "439714ae-8885-4d62-ac2b-3017dcc6d0bd",
                                Name = "Tablete"
                            },
                            new Category
                            {
                                CategoryID = "44dd5290-7b48-4fd5-bd98-497475a6c42c",
                                Name = "Laptopuri"
                            }
                        ) ;
                }

                if(!context.Products.Any())
                {
                    context.Products.AddRange
                        (
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1234",
                                Name = "Samsung Galaxy S10",
                                Price = 1999,
                                Description = "Samsung Galaxy S10 este un smartphone Android avansat si puternic, cu caracteristici multiple, un design inovator si o experienta de utilizare exceptionala.",
                                CategoryID = "3ec2a986-f612-46c5-bdce-11c6d48fbf4f",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            }
                        );
                }
                context.SaveChanges();
            }
        }
    }
}
