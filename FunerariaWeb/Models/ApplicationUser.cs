using Microsoft.AspNetCore.Identity;

namespace FunerariaWeb.Models
{
    // Estende o utilizador padrão do ASP.NET Identity com um nome próprio.
    // Os "tipos diferentes de utilizador" exigidos pelas regras são implementados
    // como Roles: "Administrador" (equipa da funerária) e "Cliente" (família).
    public class ApplicationUser : IdentityUser
    {
        public string NomeCompleto { get; set; } = string.Empty;
    }

    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Cliente = "Cliente";
    }
}
