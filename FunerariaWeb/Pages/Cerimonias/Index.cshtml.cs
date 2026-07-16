using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Cerimonias
{
    [Authorize] // Qualquer utilizador autenticado (Administrador ou Cliente) pode ver, com filtragem abaixo.
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Cerimonia> Cerimonias { get; set; } = new();

        public async Task OnGetAsync()
        {
            var query = _context.Cerimonias
                .Include(c => c.Cliente)
                .Include(c => c.CerimoniaServicos)
                .AsQueryable();

            // Controlo de acesso diferenciado: um utilizador "Cliente" só vê as suas próprias cerimónias.
            if (!User.IsInRole(Roles.Administrador))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(c => c.Cliente != null && c.Cliente.IdentityUserId == userId);
            }

            Cerimonias = await query.OrderByDescending(c => c.DataCerimonia).ToListAsync();
        }
    }
}
