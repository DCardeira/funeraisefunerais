using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Clientes
{
    [Authorize(Roles = Roles.Administrador)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Cliente> Clientes { get; set; } = new();

        public async Task OnGetAsync()
        {
            Clientes = await _context.Clientes
                .Include(c => c.Cerimonias)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
    }
}
