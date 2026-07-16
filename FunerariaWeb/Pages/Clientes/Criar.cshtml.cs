using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Clientes
{
    [Authorize(Roles = Roles.Administrador)]
    public class CriarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CriarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Evita emails duplicados (validação para além das DataAnnotations).
            if (await _context.Clientes.AnyAsync(c => c.Email == Cliente.Email))
            {
                ModelState.AddModelError("Cliente.Email", "Já existe um cliente com este email.");
                return Page();
            }

            _context.Clientes.Add(Cliente);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Cliente criado com sucesso.";
            return RedirectToPage("Index");
        }
    }
}
