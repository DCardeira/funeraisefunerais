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
    public class CriarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificadorCerimonias _notificador;

        public CriarModel(ApplicationDbContext context, NotificadorCerimonias notificador)
        {
            _context = context;
            _notificador = notificador;
        }

        [BindProperty]
        public CerimoniaFormViewModel Form { get; set; } = new();

        public List<SelectListItem> OpcoesClientes { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CarregarListasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove entradas de validação dos campos ocultos dos serviços não selecionados
            // (Nome/PrecoBase são só para reconstruir a lista, não precisam de validação própria).
            if (!ModelState.IsValid)
            {
                await CarregarListasAsync(manterSelecao: true);
                return Page();
            }

            var cerimonia = new Cerimonia
            {
                ClienteId = Form.ClienteId,
                DataCerimonia = Form.DataCerimonia,
                Local = Form.Local,
                Estado = Form.Estado,
                Observacoes = Form.Observacoes
            };

            foreach (var s in Form.ServicosDisponiveis.Where(s => s.Selecionado))
            {
                cerimonia.CerimoniaServicos.Add(new CerimoniaServico
                {
                    ServicoId = s.ServicoId,
                    Quantidade = s.Quantidade,
                    PrecoUnitario = s.PrecoBase
                });
            }

            _context.Cerimonias.Add(cerimonia);
            await _context.SaveChangesAsync();

            await _notificador.AvisarAlteracaoAsync($"Nova cerimónia criada (Id {cerimonia.Id}).");

            TempData["Mensagem"] = "Cerimónia criada com sucesso.";
            return RedirectToPage("Index");
        }

        private async Task CarregarListasAsync(bool manterSelecao = false)
        {
            OpcoesClientes = await _context.Clientes
                .OrderBy(c => c.Nome)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToListAsync();

            if (!manterSelecao)
            {
                var servicos = await _context.Servicos.OrderBy(s => s.Nome).ToListAsync();
                Form.ServicosDisponiveis = servicos.Select(s => new ServicoSelecionavel
                {
                    ServicoId = s.Id,
                    Nome = s.Nome,
                    PrecoBase = s.PrecoBase
                }).ToList();
            }
        }
    }
}
