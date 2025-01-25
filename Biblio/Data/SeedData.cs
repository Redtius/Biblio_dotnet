namespace Biblio.Data
{
    using Biblio.Models;
    using Microsoft.AspNetCore.Identity;
        public static class SeedData
        {
        public static async Task ResetAdminPasswordAsync(IServiceProvider serviceProvider)
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<Utilisateur>>();
                var adminUser = await userManager.FindByEmailAsync("admin@bibliotheque.com");

                if (adminUser != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                    var result = await userManager.ResetPasswordAsync(adminUser, token, "Admin@12345");

                    if (result.Succeeded)
                    {
                        Console.WriteLine("✅ Admin password has been reset to: Admin@12345");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"❌ {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("❌ Admin user not found.");
                }
            }
        public static async Task InitializeAsync(UserManager<Utilisateur> userManager, RoleManager<IdentityRole> roleManager)
            {
                string adminRole = "Admin";
                string adminEmail = "admin@bibliotheque.com";
                string adminPassword = "Admin@123";

                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                }

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var user = new Utilisateur
                    {
                        UserName = "admin",
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, adminRole);
                        Console.WriteLine("✅ Administrateur racine créé avec succès !");
                    }
                    else
                    {
                        Console.WriteLine("❌ Erreur lors de la création de l'administrateur racine.");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Erreur : {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ L'administrateur racine existe déjà.");
                }
            }
        }

}
