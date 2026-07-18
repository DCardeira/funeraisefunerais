using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Clientes
{
    [Authorize(Roles = Roles.Administrador)]
    public class ApagarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ApagarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = new();

        public bool TemCerimoniasAssociadas { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Cerimonias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente is null)
            {
                return RedirectToPage("/Erro/Index", new { codigo = 404 });
            }

            Cliente = cliente;
            TemCerimoniasAssociadas = cliente.Cerimonias.Any();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cliente = await _context.Clientes
                .Include(c => c.Cerimonias)
                .FirstOrDefaultAsync(c => c.Id == Cliente.Id);

            if (cliente is null)
            {
                return RedirectToPage("Index");
            }

            if (cliente.Cerimonias.Any())
            {
                TempData["Mensagem"] = "Não é possível apagar: existem cerimónias associadas a este cliente.";
                return RedirectToPage("Index");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Cliente apagado.";
            return RedirectToPage("Index");
        }
    }
}
