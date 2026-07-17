using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Servicos
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
        public Servico Servico { get; set; } = new();

        public bool EmUso { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.CerimoniaServicos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servico is null)
            {
                return RedirectToPage("/Erro/Index", new { codigo = 404 });
            }

            Servico = servico;
            EmUso = servico.CerimoniaServicos.Any();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var servico = await _context.Servicos
                .Include(s => s.CerimoniaServicos)
                .FirstOrDefaultAsync(s => s.Id == Servico.Id);

            if (servico is null)
            {
                return RedirectToPage("Index");
            }

            if (servico.CerimoniaServicos.Any())
            {
                TempData["Mensagem"] = "Não é possível apagar: este serviço está em uso.";
                return RedirectToPage("Index");
            }

            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Serviço apagado.";
            return RedirectToPage("Index");
        }
    }
}
