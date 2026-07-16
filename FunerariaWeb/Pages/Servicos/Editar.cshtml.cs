using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunerariaWeb.Pages.Servicos
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
        public Servico Servico { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico is null)
            {
                return RedirectToPage("/Erro/Index", new { codigo = 404 });
            }
            Servico = servico;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Servico).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Serviço atualizado com sucesso.";
            return RedirectToPage("Index");
        }
    }
}
