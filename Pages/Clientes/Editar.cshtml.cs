using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunerariaWeb.Pages.Clientes
{
    [Authorize(Roles = Roles.Administrador)]
    public class EditarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente is null)
            {
                return RedirectToPage("/Erro/Index", new { codigo = 404 });
            }
            Cliente = cliente;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Cliente).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                if (!_context.Clientes.Any(c => c.Id == Cliente.Id))
                {
                    return RedirectToPage("/Erro/Index", new { codigo = 404 });
                }
                throw;
            }

            TempData["Mensagem"] = "Cliente atualizado com sucesso.";
            return RedirectToPage("Index");
        }
    }
}
