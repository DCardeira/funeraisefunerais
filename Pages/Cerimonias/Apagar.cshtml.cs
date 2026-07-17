using FunerariaWeb.Data;
using FunerariaWeb.Hubs;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Cerimonias
{
    [Authorize(Roles = Roles.Administrador)]
    public class ApagarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificadorCerimonias _notificador;

        public ApagarModel(ApplicationDbContext context, NotificadorCerimonias notificador)
        {
            _context = context;
            _notificador = notificador;
        }

        [BindProperty]
        public Cerimonia Cerimonia { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cerimonia = await _context.Cerimonias.Include(c => c.Cliente).FirstOrDefaultAsync(c => c.Id == id);
            if (cerimonia is null)
            {
                return RedirectToPage("/Erro/Index", new { codigo = 404 });
            }
            Cerimonia = cerimonia;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cerimonia = await _context.Cerimonias
                .Include(c => c.CerimoniaServicos)
                .FirstOrDefaultAsync(c => c.Id == Cerimonia.Id);

            if (cerimonia is not null)
            {
                _context.CerimoniaServicos.RemoveRange(cerimonia.CerimoniaServicos);
                _context.Cerimonias.Remove(cerimonia);
                await _context.SaveChangesAsync();
                await _notificador.AvisarAlteracaoAsync($"Cerimónia Id {cerimonia.Id} foi apagada.");
            }

            TempData["Mensagem"] = "Cerimónia apagada.";
            return RedirectToPage("Index");
        }
    }
}
