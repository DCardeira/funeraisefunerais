using FunerariaWeb.Data;
using FunerariaWeb.Hubs;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Pages.Cerimonias
{
    [Authorize(Roles = Roles.Administrador)]
    public class EditarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificadorCerimonias _notificador;

        public EditarModel(ApplicationDbContext context, NotificadorCerimonias notificador)
        {
            _context = context;
            _notificador = notificador;
        }

        [BindProperty]
        public CerimoniaFormViewModel Form { get; set; } = new();

        public List<SelectListItem> OpcoesClientes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cerimonia = await _context.Cerimonias
                .Include(c => c.CerimoniaServicos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cerimonia is null)
            {
                return RedirectToPage("/Erro/Index", new { codigo = 404 });
            }

            var todosServicos = await _context.Servicos.OrderBy(s => s.Nome).ToListAsync();
            var selecionados = cerimonia.CerimoniaServicos.ToDictionary(cs => cs.ServicoId);

            Form = new CerimoniaFormViewModel
            {
                Id = cerimonia.Id,
                ClienteId = cerimonia.ClienteId,
                DataCerimonia = cerimonia.DataCerimonia,
                Local = cerimonia.Local,
                Estado = cerimonia.Estado,
                Observacoes = cerimonia.Observacoes,
                ServicosDisponiveis = todosServicos.Select(s => new ServicoSelecionavel
                {
                    ServicoId = s.Id,
                    Nome = s.Nome,
                    PrecoBase = s.PrecoBase,
                    Selecionado = selecionados.ContainsKey(s.Id),
                    Quantidade = selecionados.TryGetValue(s.Id, out var cs) ? cs.Quantidade : 1
                }).ToList()
            };

            await CarregarClientesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarClientesAsync();
                return Page();
            }

            var cerimonia = await _context.Cerimonias
                .Include(c => c.CerimoniaServicos)
                .FirstOrDefaultAsync(c => c.Id == Form.Id);

            if (cerimonia is null)
            {
                return RedirectToPage("/Erro/Index", new { codigo = 404 });
            }

            cerimonia.ClienteId = Form.ClienteId;
            cerimonia.DataCerimonia = Form.DataCerimonia;
            cerimonia.Local = Form.Local;
            cerimonia.Estado = Form.Estado;
            cerimonia.Observacoes = Form.Observacoes;

            // Substitui por completo as linhas da tabela de junção pela seleção atual.
            _context.CerimoniaServicos.RemoveRange(cerimonia.CerimoniaServicos);
            cerimonia.CerimoniaServicos.Clear();
            foreach (var s in Form.ServicosDisponiveis.Where(s => s.Selecionado))
            {
                cerimonia.CerimoniaServicos.Add(new CerimoniaServico
                {
                    CerimoniaId = cerimonia.Id,
                    ServicoId = s.ServicoId,
                    Quantidade = s.Quantidade,
                    PrecoUnitario = s.PrecoBase
                });
            }

            await _context.SaveChangesAsync();
            await _notificador.AvisarAlteracaoAsync($"Cerimónia Id {cerimonia.Id} foi atualizada.");

            TempData["Mensagem"] = "Cerimónia atualizada com sucesso.";
            return RedirectToPage("Index");
        }

        private async Task CarregarClientesAsync()
        {
            OpcoesClientes = await _context.Clientes
                .OrderBy(c => c.Nome)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToListAsync();
        }
    }
}
