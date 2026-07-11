using FunerariaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Data
{
    // Corre uma vez no arranque: aplica migrações pendentes e garante que existem
    // os 2 papéis (Administrador / Cliente) e uma conta de administrador de teste,
    // conforme exigido pelas regras ("identificação das credenciais de acesso...").
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            if (context.Database.ProviderName != null && context.Database.ProviderName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                await context.Database.MigrateAsync();
            }

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in new[] { Roles.Administrador, Roles.Cliente })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            const string adminEmail = "admin@funeraria.pt";
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NomeCompleto = "Administrador Geral",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Administrador);
                }
            }

            const string clienteEmail = "cliente@funeraria.pt";
            if (await userManager.FindByEmailAsync(clienteEmail) is null)
            {
                var clienteUser = new ApplicationUser
                {
                    UserName = clienteEmail,
                    Email = clienteEmail,
                    NomeCompleto = "Cliente Demonstração",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(clienteUser, "Cliente123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(clienteUser, Roles.Cliente);

                    if (!await context.Clientes.AnyAsync(c => c.Email == clienteEmail))
                    {
                        context.Clientes.Add(new Cliente
                        {
                            Nome = "Cliente Demonstração",
                            Email = clienteEmail,
                            Telefone = "912345678",
                            Morada = "Rua de Exemplo, 123",
                            IdentityUserId = clienteUser.Id
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
