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
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c4321",
                                Name = "Telefon mobil Apple iPhone 11, Black, 64 GB",
                                Price = 1889,
                                Description = "Cel mai rapid cip vreodata incorporat intr-un smartphone si durata de viata a bateriei indelungata iti vor da posibilitatea sa faci mai multe pe telefon cu cat mai putine incarcari.",
                                CategoryID = "3ec2a986-f612-46c5-bdce-11c6d48fbf4f",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1987",
                                Name = "Telefon mobil Apple iPhone 14, 128GB, 5G, Purple",
                                Price = 4569,
                                Description = "Realizeaza poze uimitoare in lumina slaba sau foarte puternica multumita noului sistem cu doua camere. Detectare accident, o noua functionalitate de siguranta, suna dupa ajutor daca tu nu poti.",
                                CategoryID = "3ec2a986-f612-46c5-bdce-11c6d48fbf4f",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1543",
                                Name = "Tableta SAMSUNG Galaxy Tab S8, 11\", 128GB, 8GB RAM, Wi-Fi, Pink Gold",
                                Price = 3690,
                                Description = "ÎNREGISTRARE VIDEO 4K.Camera frontală duală (Ultra Wide + Wide) etalează un câmp vizual impresionant, care oferă înregistrare video optimă 4K.",
                                CategoryID = "439714ae-8885-4d62-ac2b-3017dcc6d0bd",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1667",
                                Name = "Tableta APPLE iPad 10 (2022), 10.9\", 64GB, Wi-Fi, Pink",
                                Price = 4690,
                                Description = "Noul iPad este regandit in culori pentru a fi mai capabil, mai intuitiv si chiar mai distractiv.",
                                CategoryID = "439714ae-8885-4d62-ac2b-3017dcc6d0bd",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1452",
                                Name = "Tableta SAMSUNG Galaxy Tab A8, 10.5\", 32GB, 3GB RAM, Wi-Fi, Dark Gray",
                                Price = 3750,
                                Description = "Ecranul lat de 10,5 inchi, completat cu o ramă simetrică, care măsoară doar 10,2 mm, îți permite să rămâi complet captivat(ă) de ceea ce se află pe ecran.",
                                CategoryID = "439714ae-8885-4d62-ac2b-3017dcc6d0bd",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1339",
                                Name = "Laptop Apple MacBook Air 13-inch, True Tone, procesor Apple M1 , 8 nuclee CPU si 7 nuclee GPU, 8GB, 256GB, Space Grey, INT KB",
                                Price = 5299,
                                Description = "Cel mai subtire si mai usor notebook-ul nostru, complet transformat de cipul Apple M1. Viteza procesorului de pana la de 3,5 ori mai rapida.",
                                CategoryID = "44dd5290-7b48-4fd5-bd98-497475a6c42c",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1722",
                                Name = "Laptop ASUS VivoBook M1603QA-MB511 cu procesor AMD Ryzen 5 5600H, 16\" WUXGA, 8GB, 512GB SSD, AMD Radeon Vega 7 Graphics, No OS, Transparent Silver",
                                Price = 2599,
                                Description = "Vivobook 16X iti usureaza sarcinile de orice fel, oriunde ai fi: totul a fost imbunatatit, de la puternicul procesor AMD Ryzen™ mobile processor pana la balama cu deschidere completa la 180°, culorile moderne si designul geometric elegant. ",
                                CategoryID = "44dd5290-7b48-4fd5-bd98-497475a6c42c",
                                UserID = "2c5e174e - 3b0e - 446f - 86af483d56fd7203",
                                IsAvailable = true,
                                Stock = 4
                            },
                            new Product
                            {
                                ProductID = "44dd5290-7b48-4fd5-bd98-497123a6c1995",
                                Name = "Laptop HP 17-cn0048nq cu procesor Intel® Celeron® Processor N4020 pana la 2.80 GHz, 17.3 FHD Antiglare IPS, 4GB DDR4, 256GB PCIe SSD, Intel UHD Graphics, FreeDOS, Natural silver",
                                Price = 1999,
                                Description = "Laptopul HP 17 este conceput atent si asigura performante ridicate cu procesorul Intel, tehnologia Wi-Fi rapida si spatiul mare de stocare.",
                                CategoryID = "44dd5290-7b48-4fd5-bd98-497475a6c42c",
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
