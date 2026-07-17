using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Servicos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Servico> Servicos { get; set; } = new();

        public async Task OnGetAsync()
        {
            Servicos = await _context.Servicos.OrderBy(s => s.Categoria).ThenBy(s => s.Nome).ToListAsync();
        }
    }
}
